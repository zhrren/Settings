using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mark.Settings
{
    public class SettingsManager<T>
    {
        private string _rootPath;
        private SettingsWatcher _watcher;

        public SettingsManager(string settingsPath = "settings")
        {
            if (Path.IsPathRooted(settingsPath))
                _rootPath = settingsPath;
            else
                _rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsPath);

            _watcher = new SettingsWatcher(_rootPath);
            _watcher.Changed += _watcher_Notify;
        }

        private void _watcher_Notify(object sender, EventArgs e)
        {
            if (Changed != null) Changed(this, EventArgs.Empty);
        }

        public T Renew(string filename = "app.json")
        {
            lock (this)
            {
                var settingsPath = Combine(filename);
                return JsonMapper.ToObject<T>(File.ReadAllText(settingsPath));
            }
        }

        public string Combine(string filename)
        {
            var mode = "debug";

            var configPath = Path.Combine(_rootPath, "app.json");
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                var config = JsonMapper.ToObject<Configuration>(json);
                mode = config.Mode;
            }

            var filenames = filename.Split('.');
            if (filenames.Length != 2)
                throw new ArgumentException("应包含文件名和扩展名，如app.json", "filename");
            var newfilename = string.Format("{0}.{1}.{2}", filenames[0], mode, filenames[1]);
            return Path.Combine(_rootPath, newfilename);
        }

        public string Map(string filename)
        {
            return Path.Combine(_rootPath, filename);
        }

        public event EventHandler Changed;
    }
}
