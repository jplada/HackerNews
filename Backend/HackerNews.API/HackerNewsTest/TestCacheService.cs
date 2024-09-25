using HackerNews.Services.Interfaces;

namespace HackerNews.Test
{
    // Cache service implementation for integration test purpose
    internal class TestCacheService : ICacheService
    {
        private readonly Dictionary<string, object> _cache;
        public TestCacheService()
        {
            _cache = new Dictionary<string, object>();
        }
        public TItem? Get<TItem>(object key)
        {
            if (key is not null && _cache.ContainsKey((string)key))
            {
                var item = (TItem)_cache[(string)key];
                return item;
            }
            return default(TItem);
        }

        public void Set<TItem>(object key, TItem value)
        {
            if(key is not null && value is not null)
            {
                if (_cache.ContainsKey((string)key))
                {
                    _cache[(string)key] = value;
                }
                else
                {
                    _cache.Add(key.ToString(), value);
                }                
            }            
        }
    }
}
