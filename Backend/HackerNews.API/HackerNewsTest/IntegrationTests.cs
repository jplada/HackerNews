using HackerNews.API.Controllers;
using HackerNews.Services.Interfaces;
using HackerNews.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HackerNews.Test
{
    public class IntegrationTests
    {
        // Run features from controllers to HAckerNews API calls with actual implementations
        INewsService newsService { get; set; }
        public IntegrationTests()
        {
            newsService = Provider().GetService<INewsService>();
            newsService.LoadLatestNewsInCache().GetAwaiter().GetResult();
        }
        [Fact]
        public async Task Latest_ReturnsData()
        {
            NewsController controller = new NewsController(newsService);
            var result = await controller.Latest();
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
        }

        [Fact]
        public async Task Search_ReturnsData()
        {
            NewsController controller = new NewsController(newsService);
            var result = await controller.Search("the");
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();
            services.AddHttpClient<IHackerNewsServiceAgent, HackerNewsServiceAgent>();
            services.AddSingleton<ICacheService, TestCacheService>();
            services.AddScoped<IHackerNewsServiceAgent, HackerNewsServiceAgent>();
            services.AddScoped<INewsService, NewsService>();
            var myConfiguration = new Dictionary<string, string>
            {
                {"HackerNewsUrls:GetItem", "https://hacker-news.firebaseio.com/v0/item/{itemID}.json?print=pretty"},
                {"HackerNewsUrls:GetLatest", "https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
            services.AddSingleton<IConfiguration>(configuration);
            return services.BuildServiceProvider();
        }
    }
}
