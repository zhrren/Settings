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
        private Mark.FileWatcher.FileWatcher _hostingWatcher;
        private event Action _changed;

        public Hosting Hosting { get; private set; }

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

        public SettingsContext(string settingsFileName = "app.json", string settingsPath = "settings")
        {
            if (string.IsNullOrWhiteSpace(settingsFileName))
                throw new ArgumentNullException("filename");

            if (string.IsNullOrWhiteSpace(settingsPath))
                throw new ArgumentNullException("settingsPath");

            _settingsFilePath = settingsFileName;

            if (Path.IsPathRooted(settingsPath))
                _rootPath = settingsPath;
            else
                _rootPath = Path.Combine(AppContext.BaseDirectory, settingsPath);

            var hostingPath = Path.Combine(_rootPath, "hosting.json");
            if (File.Exists(hostingPath))
            {
                _hostingWatcher = new FileWatcher.FileWatcher(hostingPath);
                _hostingWatcher.AddChangedListener(hostingWatcher_Changed, true);
            }
            else
            {
                Hosting = new Hosting();
                var settingsFilePath = Combine(_settingsFilePath);
                _settingsWatcher = new Mark.FileWatcher.FileWatcher(settingsFilePath);
                _settingsWatcher.AddChangedListener(settingsWatcher, true);
            }

        }
        private void hostingWatcher_Changed(FileSystemEventArgs e)
        {
            var content = File.ReadAllText(e.FullPath);
            Hosting = JsonConvert.DeserializeObject<Hosting>(content);

            if (Hosting == null)
                throw new ArgumentException("Settings file invalid: " + e.FullPath);

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

            if (_changed != null)
                _changed();
        }

        public string Combine(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            var filenames = filename.Split('.');
            if (filenames.Length != 2)
                throw new ArgumentException("filename");

            var newfilename = string.Format("{0}.{1}.{2}", filenames[0], Hosting.EnvironmentName.ToLower(), filenames[1]);
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
            var settingsPath = Combine(_settingsFilePath);
            var content = File.ReadAllText(settingsPath);
            return JsonConvert.DeserializeObject(content);
        }

        public T Renew<T>()
        {
            var settingsPath = Combine(_settingsFilePath);
            var content = File.ReadAllText(settingsPath);
            return JsonConvert.DeserializeObject<T>(content);
        }

        public void Dispose()
        {
            if (_hostingWatcher != null)
            {
                _hostingWatcher.Changed -= hostingWatcher_Changed;
                _hostingWatcher.Dispose();
                _hostingWatcher = null;
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
