using Master_BLL.Services.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{
    public class MemoryCacheRepository : IMemoryCacheRepository
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            
        }
        public async Task<T?> GetCahceKey<T>(string cachKey)
        {
            return await Task.FromResult(_memoryCache.TryGetValue(cachKey, out T? value) ? value : default(T));
        }

        public Task RemoveAsync(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
            return Task.CompletedTask;
        }

        public async Task SetAsync<T>(string cacheKey, T value, MemoryCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            if(value is not null)
            {
                _memoryCache.Set(cacheKey, value, options);
            }
            await Task.CompletedTask;
        }
    }
}
