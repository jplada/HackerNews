using HackerNews.Services.Interfaces;
using HackerNews.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("Search")]
        public async Task<PagedResponseDTO<IEnumerable<NewsItem>>> Search(string? searchTerm = null, int pageNumber = 0, int pageSize = 20)
        {
            searchTerm = searchTerm ?? string.Empty;
            var news = await _newsService.Search(searchTerm, pageNumber, pageSize);
            return news;
        }
    }
}
