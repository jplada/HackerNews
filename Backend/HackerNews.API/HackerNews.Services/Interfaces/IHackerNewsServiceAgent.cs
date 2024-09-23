using HackerNews.Services.Models.HackerNews;

namespace HackerNews.Services.Interfaces
{
    /// <summary>
    /// Service to get data from Hacker News API
    /// </summary>
    public interface IHackerNewsServiceAgent
    {
        Task<IEnumerable<int>> GetLatest();
        Task<Item> GetItem(int id);
    }
}
