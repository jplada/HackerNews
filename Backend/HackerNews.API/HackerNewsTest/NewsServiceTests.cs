using HackerNews.Services.Interfaces;
using HackerNews.Services.Models;
using HackerNews.Services.Models.HackerNews;
using HackerNews.Services.Services;
using HackerNews.Services.Utility;
using Moq;

namespace HackerNews.Test
{
    public class NewsServiceTests
    {
        [Fact]
        public async Task GetLatest_ReturnsFirstPageCorrectly()
        {
            var newsItems = MockedItems();
            List<NewsItem> cachedItems = newsItems.Where(i => i.id < 1006).Select(i => new NewsItem { Id = i.id, Title = i.title }).ToList();
            var serviceAgentMock = new Mock<IHackerNewsServiceAgent>();
            serviceAgentMock.Setup(m => m.GetLatest()).ReturnsAsync(newsItems.Select(i => i.id)).Verifiable();
            foreach (var item in newsItems)
            {
                serviceAgentMock.Setup(m => m.GetItem(It.Is<int>(x => x == item.id))).ReturnsAsync(newsItems.First(i => i.id == item.id));
            }

            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(mc => mc.Get<List<NewsItem>>(It.Is<string>(x => x == Constants.NewsItemsKey))).Returns(cachedItems).Verifiable();
            cacheServiceMock.Setup(mc => mc.Set(It.Is<string>(x => x == Constants.NewsItemsKey),
                    It.Is<List<NewsItem>>(x => x.Count()==10))).Verifiable();

            var service = new NewsService(serviceAgentMock.Object, cacheServiceMock.Object);

            var result = await service.GetLatest(0, 10);

            Assert.NotNull(result);
            Assert.Equal(0, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.NotNull(result.Data);
            Assert.Equal(10, result.Data.Count());
            Assert.Equal(1001, result.Data.First().Id);
            Assert.Equal(1010, result.Data.Last().Id);
            serviceAgentMock.Verify();
            cacheServiceMock.Verify();
        }

        [Fact]
        public async Task GetLatest_ReturnsSecondPageCorrectly()
        {
            var newsItems = MockedItems();
            List<NewsItem> cachedItems = newsItems.Where(i => i.id < 1006).Select(i => new NewsItem { Id = i.id, Title = i.title }).ToList();
            var serviceAgentMock = new Mock<IHackerNewsServiceAgent>();
            serviceAgentMock.Setup(m => m.GetLatest()).ReturnsAsync(newsItems.Select(i => i.id)).Verifiable();
            foreach (var item in newsItems)
            {
                serviceAgentMock.Setup(m => m.GetItem(It.Is<int>(x => x == item.id))).ReturnsAsync(newsItems.First(i => i.id == item.id)).Verifiable();
            }
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(mc => mc.Get<List<NewsItem>>(It.Is<string>(x => x == Constants.NewsItemsKey))).Returns(cachedItems).Verifiable();
            cacheServiceMock.Setup(mc => mc.Set(It.Is<string>(x => x == Constants.NewsItemsKey),
                    It.Is<List<NewsItem>>(x => x.Count() == 10))).Verifiable();

            var service = new NewsService(serviceAgentMock.Object, cacheServiceMock.Object);

            var result = await service.GetLatest(1, 10);

            Assert.NotNull(result);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.NotNull(result.Data);
            Assert.Equal(5, result.Data.Count());
            Assert.Equal(1011, result.Data.First().Id);
            Assert.Equal(1015, result.Data.Last().Id);
        }

        [Fact]
        public async Task GetLatest_ReturnsEmptyPageCorrectly()
        {
            var newsItems = MockedItems();
            List<NewsItem> cachedItems = newsItems.Where(i => i.id < 1006).Select(i => new NewsItem { Id = i.id, Title = i.title }).ToList();
            var serviceAgentMock = new Mock<IHackerNewsServiceAgent>();
            serviceAgentMock.Setup(m => m.GetLatest()).ReturnsAsync(newsItems.Select(i => i.id)).Verifiable();
            foreach (var item in newsItems)
            {
                serviceAgentMock.Setup(m => m.GetItem(It.Is<int>(x => x == item.id))).ReturnsAsync(newsItems.First(i => i.id == item.id));
            }
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(mc => mc.Get<List<NewsItem>>(It.Is<string>(x => x == Constants.NewsItemsKey))).Returns(cachedItems);

            var service = new NewsService(serviceAgentMock.Object, cacheServiceMock.Object);

            var result = await service.GetLatest(3, 10);

            Assert.NotNull(result);
            Assert.Equal(3, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
            serviceAgentMock.Verify();
        }

        [Fact]
        public async Task Search_ReturnsFirstPageCorrectly()
        {
            var newsItems = MockedItems();
            List<NewsItem> cachedItems = newsItems.Where(i => i.id < 1006).Select(i => new NewsItem { Id = i.id, Title = i.title }).ToList();
            var serviceAgentMock = new Mock<IHackerNewsServiceAgent>();
            serviceAgentMock.Setup(m => m.GetLatest()).ReturnsAsync(newsItems.Select(i => i.id)).Verifiable();
            foreach (var item in newsItems)
            {
                serviceAgentMock.Setup(m => m.GetItem(It.Is<int>(x => x == item.id))).ReturnsAsync(newsItems.First(i => i.id == item.id));
            }

            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(mc => mc.Get<List<NewsItem>>(It.Is<string>(x => x == Constants.NewsItemsKey))).Returns(cachedItems).Verifiable();
            cacheServiceMock.Setup(mc => mc.Set(It.Is<string>(x => x == Constants.NewsItemsKey),
                    It.Is<List<NewsItem>>(x => x.Count() == 15))).Verifiable();

            var service = new NewsService(serviceAgentMock.Object, cacheServiceMock.Object);

            var result = await service.Search("NetCore", 0, 10);

            int[] expectedNews = [1002,1004,1006,1007,1008,1009,1011,1012];
            Assert.NotNull(result);
            Assert.Equal(0, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.NotNull(result.Data);
            Assert.Equal(8, result.Data.Count());
            Assert.True(result.Data.All(x => expectedNews.Contains(x.Id)));
            serviceAgentMock.Verify();
            cacheServiceMock.Verify();
        }

        [Fact]
        public async Task Search_ReturnsSecondPageCorrectly()
        {
            var newsItems = MockedItems();
            List<NewsItem> cachedItems = newsItems.Where(i => i.id < 1006).Select(i => new NewsItem { Id = i.id, Title = i.title }).ToList();
            var serviceAgentMock = new Mock<IHackerNewsServiceAgent>();
            serviceAgentMock.Setup(m => m.GetLatest()).ReturnsAsync(newsItems.Select(i => i.id)).Verifiable();
            foreach (var item in newsItems)
            {
                serviceAgentMock.Setup(m => m.GetItem(It.Is<int>(x => x == item.id))).ReturnsAsync(newsItems.First(i => i.id == item.id));
            }

            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(mc => mc.Get<List<NewsItem>>(It.Is<string>(x => x == Constants.NewsItemsKey))).Returns(cachedItems).Verifiable();
            cacheServiceMock.Setup(mc => mc.Set(It.Is<string>(x => x == Constants.NewsItemsKey),
                    It.Is<List<NewsItem>>(x => x.Count() == 15))).Verifiable();

            var service = new NewsService(serviceAgentMock.Object, cacheServiceMock.Object);

            var result = await service.Search("NetCore", 1, 5);

            int[] expectedNews = [1009, 1011, 1012];
            Assert.NotNull(result);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.TotalPages);
            Assert.NotNull(result.Data);
            Assert.Equal(3, result.Data.Count());
            Assert.True(result.Data.All(x => expectedNews.Contains(x.Id)));
            serviceAgentMock.Verify();
            cacheServiceMock.Verify();
        }


        private IEnumerable<Item> MockedItems()
        {
            List<Item> items = [
                new Item {
                    id = 1001,
                    title = "Title1",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1002,
                    title = "What's new in Netcore 8",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1003,
                    title = "Javascript fundamentals",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1004,
                    title = "Netcore WebAPI best practices",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1005,
                    title = "Intro to Angular (Javascript)",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1006,
                    title = "Entity Framework with NetCore 8",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1007,
                    title = "Development Market for Netcore Java and Node",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1008,
                    title = "Multithreadin in NetCore",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1009,
                    title = "What is Blazor, new Netcore front tool",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1010,
                    title = "Angular versus React, a comparison (javascript)",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1011,
                    title = "Extensions methods in Netcore",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1012,
                    title = "Microservices implementation with Netcore",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1013,
                    title = "Title13",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1014,
                    title = "Title14",
                    url = "https://github.com/abc/title"
                },
                new Item {
                    id = 1015,
                    title = "Title15",
                    url = "https://github.com/abc/title"
                }
            ];
            return items;
        }
    }
}