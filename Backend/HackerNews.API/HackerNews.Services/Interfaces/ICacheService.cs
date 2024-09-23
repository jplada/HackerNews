namespace HackerNews.Services.Interfaces
{
    /// <summary>
    /// Wrapper over IMemoryCache to allow mock cache in unit tests
    /// </summary>
    public interface ICacheService
    {
        void Set<TItem>(object key, TItem value);
        TItem? Get<TItem>(object key);
    }
}
