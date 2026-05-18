using System;
using System.IO;

namespace FileContentToolkit.Watcher
{
    // Debounced FileSystemWatcher: bursts of events collapse into one Changed event
    // after activity stops for `debounceMs` ms.
    public sealed class FolderWatcher : IDisposable
    {
        private FileSystemWatcher? _fsw;
        private System.Threading.Timer? _debounce;
        private readonly int _debounceMs;

        public event Action? Changed;

        public bool IsRunning => _fsw != null;

        public FolderWatcher(int debounceMs = 600)
        {
            _debounceMs = debounceMs;
        }

        public void Start(string folder, bool includeSubfolders)
        {
            Stop();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return;

            _fsw = new FileSystemWatcher(folder)
            {
                IncludeSubdirectories = includeSubfolders,
                NotifyFilter = NotifyFilters.FileName
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Size,
                EnableRaisingEvents = true,
            };

            _fsw.Changed += OnFsEvent;
            _fsw.Created += OnFsEvent;
            _fsw.Deleted += OnFsEvent;
            _fsw.Renamed += OnFsEvent;
            _fsw.Error += (s, e) => { /* swallow */ };

            _debounce = new System.Threading.Timer(_ =>
            {
                try { Changed?.Invoke(); } catch { /* swallow */ }
            }, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        public void Stop()
        {
            if (_fsw != null)
            {
                try { _fsw.EnableRaisingEvents = false; } catch { }
                _fsw.Dispose();
                _fsw = null;
            }
            _debounce?.Dispose();
            _debounce = null;
        }

        public void Dispose() => Stop();

        private void OnFsEvent(object sender, FileSystemEventArgs e)
        {
            _debounce?.Change(_debounceMs, System.Threading.Timeout.Infinite);
        }
    }
}
