using HackerNews.Services.Interfaces;
using HackerNews.Services.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Services.Services
{
    /// <summary>
    /// Wrapper over IMemoryCache to allow mock cache in unit tests
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public TItem? Get<TItem>(object key)
        {
            return _memoryCache.Get<TItem>(key);
        }

        public void Set<TItem>(object key, TItem value)
        {
            _memoryCache.Set(key, value);
        }
    }
}
