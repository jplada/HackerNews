using HackerNews.Services.Interfaces;
using HackerNews.Services.Models;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("Latest")]
        public async Task<PagedResponseDTO<IEnumerable<NewsItem>>> Latest(int pageNumber = 0, int pageSize = 20)
        {
            var news = await _newsService.GetLatest(pageNumber, pageSize);
            return news;
        }

        [HttpGet("Search")]
        public async Task<PagedResponseDTO<IEnumerable<NewsItem>>> Search(string searchTerm, int pageNumber = 0, int pageSize = 20)
        {
            var news = await _newsService.Search(searchTerm, pageNumber, pageSize);
            return news;
        }
    }
}
