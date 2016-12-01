using Mark.FileWatcher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mark.SettingsContext
{
    public class SettingsContext
    {
        private string _rootPath;
        private string _settingsFilePath;
        private dynamic _data;

        public string Env { get; private set; }

        public dynamic Data
        {
            get
            {
                if (_data == null)
                    lock (this)
                    {
                        _data = Renew();
                    }
                return _data;
            }
        }

        public SettingsContext(string filename = "app.json", string settingsPath = "settings") :
            this(null, filename, settingsPath)
        { }

        public SettingsContext(Action renewHandler, string filename = "app.json", string settingsPath = "settings")
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            if (string.IsNullOrWhiteSpace(settingsPath))
                throw new ArgumentNullException("settingsPath");

            _settingsFilePath = filename;

            if (Path.IsPathRooted(settingsPath))
                _rootPath = settingsPath;
            else
                _rootPath = Path.Combine(AppContext.BaseDirectory, settingsPath);

            Env = "development";

            var configPath = Path.Combine(_rootPath, "app.json");
            if (File.Exists(configPath))
            {
                var content = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<Configuration>(content);
                Env = config.Env;
            }

            FileWatcherManager.Watch(_rootPath, (args) =>
            {
                lock (this)
                {
                    _data = null;
                }
                renewHandler?.Invoke();
            }, true);
        }

        public string Combine(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            var filenames = filename.Split('.');
            if (filenames.Length != 2)
                throw new ArgumentException("filename");

            var newfilename = string.Format("{0}.{1}.{2}", filenames[0], Env, filenames[1]);
            return Path.Combine(_rootPath, newfilename);
        }

        public string Map(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException(filename);

            return Path.Combine(_rootPath, filename);
        }

        public dynamic Renew()
        {
            lock (_settingsFilePath)
            {
                var settingsPath = Combine(_settingsFilePath);
                var content = File.ReadAllText(settingsPath);
                return JsonConvert.DeserializeObject(content);
            }
        }

        public T Renew<T>()
        {
            lock (_settingsFilePath)
            {
                var settingsPath = Combine(_settingsFilePath);
                var content = File.ReadAllText(settingsPath);
                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}
