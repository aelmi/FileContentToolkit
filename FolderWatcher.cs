using System;
using System.IO;
using System.Threading;

namespace CodeShuttle.Watcher
{
    /// <summary>
    /// Debounced FileSystemWatcher: bursts of events collapse into one Changed event after
    /// activity stops for <c>debounceMs</c>.
    ///
    /// Three failure modes are handled explicitly. The internal buffer overflows routinely during
    /// a build or an npm install, after which the watcher delivers nothing ever again — so Error
    /// triggers a restart with backoff instead of being swallowed. Events are only enabled once
    /// the handlers and the debounce timer exist. And Stop() cannot race a callback that is
    /// already in flight.
    /// </summary>
    public sealed class FolderWatcher : IDisposable
    {
        private const int BufferSize = 64 * 1024;
        private const int MaxRestartAttempts = 5;

        private readonly object _gate = new();
        private readonly int _debounceMs;

        private FileSystemWatcher? _fsw;
        private System.Threading.Timer? _debounce;
        private CancellationTokenSource? _cts;
        private string _folder = "";
        private bool _includeSubfolders;
        private int _restartAttempts;
        private bool _disposed;

        public event Action? Changed;

        /// <summary>Raised when watching has failed permanently, so the UI can untick the checkbox.</summary>
        public event Action<string>? Failed;

        public bool IsRunning
        {
            get { lock (_gate) return _fsw != null; }
        }

        public FolderWatcher(int debounceMs = 600)
        {
            _debounceMs = debounceMs;
        }

        public void Start(string folder, bool includeSubfolders)
        {
            Stop();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return;

            lock (_gate)
            {
                if (_disposed) return;
                _folder = folder;
                _includeSubfolders = includeSubfolders;
                _restartAttempts = 0;
                StartCore();
            }
        }

        private void StartCore()
        {
            // Caller holds _gate.
            var cts = new CancellationTokenSource();
            _cts = cts;
            var token = cts.Token;

            _debounce = new System.Threading.Timer(_ =>
            {
                if (token.IsCancellationRequested) return;
                try { Changed?.Invoke(); } catch { /* a subscriber's failure must not kill the watcher */ }
            }, null, Timeout.Infinite, Timeout.Infinite);

            var fsw = new FileSystemWatcher(_folder)
            {
                IncludeSubdirectories = _includeSubfolders,
                InternalBufferSize = BufferSize,
                NotifyFilter = NotifyFilters.FileName
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Size
            };

            fsw.Changed += OnFsEvent;
            fsw.Created += OnFsEvent;
            fsw.Deleted += OnFsEvent;
            fsw.Renamed += OnFsEvent;
            fsw.Error += OnError;

            _fsw = fsw;

            // Enabled LAST: with it set in the initialiser, events could arrive before the
            // handlers were attached and before _debounce existed.
            try
            {
                fsw.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                _fsw = null;
                fsw.Dispose();
                Failed?.Invoke(ex.Message);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            string? message = null;
            lock (_gate)
            {
                if (_disposed) return;
                StopCore();

                if (_restartAttempts >= MaxRestartAttempts)
                {
                    message = e.GetException()?.Message ?? "The folder watcher stopped responding.";
                }
                else
                {
                    _restartAttempts++;
                }
            }

            if (message != null)
            {
                Failed?.Invoke(message);
                return;
            }

            // Exponential backoff: 200ms, 400ms, 800ms, …
            int attempt;
            lock (_gate) attempt = _restartAttempts;
            var delay = TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1));

            _ = new System.Threading.Timer(_ =>
            {
                lock (_gate)
                {
                    if (_disposed || _folder.Length == 0) return;
                    try { StartCore(); }
                    catch { /* the next Error will retry or give up */ }
                }
            }, null, delay, Timeout.InfiniteTimeSpan);
        }

        public void Stop()
        {
            lock (_gate) StopCore();
        }

        private void StopCore()
        {
            // Caller holds _gate. Cancel first so a debounce callback that is already running
            // observes the cancellation instead of firing against a torn-down watcher.
            _cts?.Cancel();

            if (_fsw != null)
            {
                try { _fsw.EnableRaisingEvents = false; } catch { }
                _fsw.Changed -= OnFsEvent;
                _fsw.Created -= OnFsEvent;
                _fsw.Deleted -= OnFsEvent;
                _fsw.Renamed -= OnFsEvent;
                _fsw.Error -= OnError;
                try { _fsw.Dispose(); } catch { }
                _fsw = null;
            }

            try { _debounce?.Dispose(); } catch { }
            _debounce = null;

            _cts?.Dispose();
            _cts = null;
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _disposed = true;
                StopCore();
            }
        }

        private void OnFsEvent(object sender, FileSystemEventArgs e)
        {
            lock (_gate)
            {
                _debounce?.Change(_debounceMs, Timeout.Infinite);
            }
        }
    }
}
