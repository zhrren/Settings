using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mark.Settings
{ 
    internal class SettingsWatcher
    {
        private Timer _timer;
        private const int TimeoutMillis = 500;
        private FileSystemWatcher watcher;

        public SettingsWatcher(string path)
        {
            watcher = new FileSystemWatcher();
            watcher.Filter = "*.*";
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;
            watcher.Changed += OnWatcherChanged;
            watcher.Created += OnWatcherChanged;
            watcher.Deleted += OnWatcherChanged;
            watcher.Renamed += OnWatcherRenamed;
            watcher.EnableRaisingEvents = true;
            _timer = new Timer(new TimerCallback(OnTimerCallback), null, Timeout.Infinite, Timeout.Infinite);
        }

        void OnWatcherRenamed(object sender, RenamedEventArgs e)
        {
            _timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            _timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        private void OnTimerCallback(object state)
        {
            if (Changed != null)
                Changed(this, EventArgs.Empty);
        }

        public event EventHandler Changed;
    }
}
