using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace cache_test
{
    public class MemoryCacheService:BackgroundService
    {
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(ILogger<MemoryCacheService> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"MemoryCacheService is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation($"MemoryCacheService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_memoryCache.TryGetValue(MemoryCacheKey.StoredDateTime, out int data))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; 
                    _logger.LogInformation(
                        $"Current cache value is:{data.ToString()}");
                    Console.ForegroundColor = ConsoleColor.White; 
                }
                
                _logger.LogInformation($"MemoryCacheService task doing background work.");

                _memoryCache.Set(MemoryCacheKey.StoredDateTime, ++data);

                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            _logger.LogInformation($"MemoryCacheService background task is stopping.");
        }

        public override void Dispose()
        {
            _memoryCache.Dispose();
        }
    }
}