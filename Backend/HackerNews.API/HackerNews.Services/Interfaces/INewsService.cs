using HackerNews.Services.Models;

namespace HackerNews.Services.Interfaces
{
    /// <summary>
    /// Hacker News Business logic
    /// </summary>
    public interface INewsService
    {
        Task LoadLatestNewsInCache();
        Task<PagedResponseDTO<IEnumerable<NewsItem>>> Search(string searchTerm, int pageNumber, int pageSize);
    }
}
