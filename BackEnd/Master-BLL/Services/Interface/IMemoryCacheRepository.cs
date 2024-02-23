using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IMemoryCacheRepository
    {
        Task<T?> GetCahceKey<T>(string cachKey);
        Task SetAsync<T>(string cacheKey, T value, MemoryCacheEntryOptions options);
        Task RemoveAsync(string cacheKey);
    }
}
