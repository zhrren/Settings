using Mark.FileWatcher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mark.SettingsManager
{
    public class SettingsManager<T>
    {
        private string _rootPath;
        private string _settingsFilePath;
        private string _env;

        public SettingsManager(string filename = "app.json", string settingsPath = "settings"):
            this(null, filename, settingsPath)
        { }

        public SettingsManager(Action<T> renewHandler, string filename = "app.json", string settingsPath = "settings")
        {
            _settingsFilePath = filename;

            if (Path.IsPathRooted(settingsPath))
                _rootPath = settingsPath;
            else
                _rootPath = Path.Combine(AppContext.BaseDirectory, settingsPath);

            _env = "development";

            var configPath = Path.Combine(_rootPath, "app.json");
            if (File.Exists(configPath))
            {
                var content = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<Configuration>(content);
                _env = config.Env;
            }

            if (renewHandler != null)
            {
                FileWatcherManager.Watch(_rootPath, (args) =>
                {
                    renewHandler(Renew());
                }, true);
            }
        }

        public string Env { get { return _env; } }

        public T Renew()
        {
            lock (this)
            {
                var settingsPath = Combine(_settingsFilePath);
                var content = File.ReadAllText(settingsPath);
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        public string Combine(string filename)
        {
            var filenames = filename.Split('.');
            if (filenames.Length != 2)
                throw new ArgumentException("应包含文件名和扩展名，如app.json", "filename");
            var newfilename = string.Format("{0}.{1}.{2}", filenames[0], _env, filenames[1]);
            return Path.Combine(_rootPath, newfilename);
        }

        public string Map(string filename)
        {
            return Path.Combine(_rootPath, filename);
        }
    }
}
