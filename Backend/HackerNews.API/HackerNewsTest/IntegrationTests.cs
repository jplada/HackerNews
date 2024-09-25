using HackerNews.API.Controllers;
using HackerNews.Services.Interfaces;
using HackerNews.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HackerNews.Test
{
    [Collection("NewsServiceFixture collection")]
    public class IntegrationTests
    {
        // Utility object that is created once for all unit tests
        NewsServiceFixture fixture;

        public IntegrationTests(NewsServiceFixture fixture)
        {
            this.fixture = fixture;            
        }

        [Fact]
        public async Task Search_ReturnsData()
        {
            NewsController controller = new NewsController(fixture.NewsService);
            var result = await controller.Search("the");
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task Search_EmptySearchTerm_ReturnsData()
        {
            NewsController controller = new NewsController(fixture.NewsService);
            var result = await controller.Search(null, 0, 10);
            Assert.NotNull(result);
            Assert.True(result.Success);
        }


    }

    public class NewsServiceFixture : IDisposable
    {
        public INewsService NewsService { get; set; }
        public NewsServiceFixture()
        {
            NewsService = Provider().GetService<INewsService>();
            NewsService.LoadLatestNewsInCache().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
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

    [CollectionDefinition("NewsServiceFixture collection")]
    public class ServiceProviderCollection : ICollectionFixture<NewsServiceFixture>
    {
    }
}
