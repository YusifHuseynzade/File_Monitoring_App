using Domain;
using Domain.Models;

namespace Core.Services
{
    public sealed class FileMonitoringService : IDisposable
    {
        private readonly MonitoringSettings _settings;
        private readonly IFileLoaderFactory _loaderFactory;
        private readonly HashSet<string> _processedFiles;
        private readonly Timer _timer;
        private readonly object _lock = new object();
        private volatile bool _isChecking;

        public event Action<List<Trade>> NewDataLoaded;

        public FileMonitoringService(MonitoringSettings settings, IFileLoaderFactory loaderFactory)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _loaderFactory = loaderFactory ?? throw new ArgumentNullException(nameof(loaderFactory));

            _processedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _timer = new Timer(TimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public void Start()
        {
            try
            {
                Directory.CreateDirectory(_settings.InputDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating input directory {_settings.InputDirectory}: {ex.Message}");
                return;
            }

            _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_settings.MonitoringFrequencySeconds));
        }

        public void Stop()
        {
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private async void TimerCallback(object state)
        {
            if (_isChecking)
            {
                return;
            }

            try
            {
                _isChecking = true;
                await CheckForNewFilesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during file monitoring: {ex.Message}");
            }
            finally
            {
                _isChecking = false;
            }
        }

        private Task CheckForNewFilesAsync()
        {
            var filesToProcess = Directory.EnumerateFiles(_settings.InputDirectory)
                                          .Where(file => !_processedFiles.Contains(file))
                                          .ToList();

            if (!filesToProcess.Any())
            {
                return Task.CompletedTask;
            }

            var tasks = filesToProcess.Select(ProcessFileAsync).ToList();
            return Task.WhenAll(tasks);
        }

        private async Task ProcessFileAsync(string filePath)
        {
            bool added;
            lock (_lock)
            {
                added = _processedFiles.Add(filePath);
            }

            if (!added)
            {
                return;
            }

            IFileLoader loader = _loaderFactory.GetLoader(filePath);
            if (loader == null)
            {
                return;
            }

            try
            {
                List<Trade> trades = await loader.LoadAsync(filePath);
                if (trades != null && trades.Any())
                {
                    NewDataLoaded?.Invoke(trades);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process file {filePath}: {ex.Message}");

                lock (_lock)
                {
                    _processedFiles.Remove(filePath);
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}