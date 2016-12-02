using Mark.FileWatcher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mark.SettingsContext
{
    public class SettingsContext : IDisposable
    {
        public static SettingsContext DEFAULT = new SettingsContext();

        private string _rootPath;
        private string _settingsFilePath;
        private dynamic _data;
        private Mark.FileWatcher.FileWatcher _settingsWatcher;
        private Mark.FileWatcher.FileWatcher _envWatcher;
        private event Action _changed;

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

        public SettingsContext(string filename = "app.json", string settingsPath = "settings")
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
                _envWatcher = new FileWatcher.FileWatcher(configPath);
                _envWatcher.AddChangedListener(envWatcher_Changed, true);
            }

        }
        private void envWatcher_Changed(FileSystemEventArgs e)
        {
            var content = File.ReadAllText(e.FullPath);
            var config = JsonConvert.DeserializeObject<Configuration>(content);
            Env = config.Env;

            if (_settingsWatcher != null)
            {
                _settingsWatcher.Dispose();
            }

            var settingsPath = Combine(_settingsFilePath);
            _settingsWatcher = new Mark.FileWatcher.FileWatcher(settingsPath);
            _settingsWatcher.AddChangedListener(settingsWatcher, true);
        }

        private void settingsWatcher(FileSystemEventArgs e)
        {
            lock (this)
            {
                _data = null;
            }
            _changed?.Invoke();
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

        public void Dispose()
        {
            if (_envWatcher != null)
            {
                _envWatcher.Changed -= envWatcher_Changed;
                _envWatcher.Dispose();
                _envWatcher = null;
            }
            if (_settingsWatcher != null)
            {
                _settingsWatcher.Changed -= settingsWatcher;
                _settingsWatcher.Dispose();
                _settingsWatcher = null;
            }
            _changed = null;
        }

        public void AddChangedListener(Action listener, bool immediate = false)
        {
            Changed += listener;
            if (immediate)
                listener();
        }

        public event Action Changed;
    }
}
