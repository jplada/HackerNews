using HackerNews.Services.Interfaces;
using HackerNews.Services.Models;
using HackerNews.Services.Utility;
using System.Collections.Concurrent;

namespace HackerNews.Services.Services
{
    public class NewsService : INewsService
    {
        private readonly IHackerNewsServiceAgent _serviceAgent;
        private readonly ICacheService cacheService;
        public NewsService(IHackerNewsServiceAgent serviceAgent,
            ICacheService memoryCache)
        {
            _serviceAgent = serviceAgent;
            cacheService = memoryCache;
        }

        /// <summary>
        /// Get all latest news items from API and store in cache
        /// </summary>
        /// <returns></returns>
        public async Task LoadLatestNewsInCache()
        {
            var newsItems = await _serviceAgent.GetLatest();
            ConcurrentBag<NewsItem> itemsResult = new ConcurrentBag<NewsItem>();
            await Parallel.ForEachAsync(newsItems, async (newsItem, cancellationToken) =>
            {
                var item = await _serviceAgent.GetItem(newsItem);
                if (item is not null)
                {
                    itemsResult.Add(new NewsItem
                    {
                        Id = item.id,
                        Title = item.title,
                        Url = item.url,
                    });
                }
            });
            cacheService.Set(Constants.NewsItemsKey, itemsResult.ToList());
        }

        public async Task<PagedResponseDTO<IEnumerable<NewsItem>>> Search(string searchTerm, int pageNumber, int pageSize)
        {
            ValidateParameters(pageNumber, pageSize);
            var latestNewsItems = await GetNewsItems();
            var pagedNewsItems = ApplySearchTermAndPaging(searchTerm, pageNumber, pageSize, latestNewsItems, out int totalItems);
            return new PagedResponseDTO<IEnumerable<NewsItem>>
            {
                Data = pagedNewsItems,
                CurrentPage = pageNumber,
                TotalPages = ((totalItems + pageSize - 1) / pageSize),
                Success = true
            };
        }

        private void ValidateParameters(int pageNumber, int pageSize)
        {
            if (pageNumber < 0 || pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("Page number / page size are not valid");
            }
        }

        private async Task<List<NewsItem>> GetNewsItems()
        {
            // Get Latest from external API
            var latestNewsItemIds = await _serviceAgent.GetLatest();
            // Get news currently stored in cache
            var cacheNewsItems = cacheService.Get<List<NewsItem>>(Constants.NewsItemsKey);
            // Verify every item is in cache and get from external API those that aren't
            cacheNewsItems = await EnsureAllItemsInCache(latestNewsItemIds, cacheNewsItems);
            // Set items from cache in same order as latest list
            List<NewsItem> orderedItems = new List<NewsItem>();
            foreach (var id in latestNewsItemIds)
            {
                var newsItem = cacheNewsItems.FirstOrDefault(ni => ni.Id == id);
                if (newsItem != null)
                {
                    orderedItems.Add(newsItem);
                }
            }
            return orderedItems;
        }

        private IEnumerable<NewsItem> ApplySearchTermAndPaging(string searchTerm, int pageNumber, int pageSize, 
            List<NewsItem> latestNewsItems, out int totalItems)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Search term in title
                latestNewsItems = latestNewsItems.Where(ni => ni.Title!=null && ni.Title
                    .Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            totalItems = latestNewsItems.Count;
            // Take paged items that match search
            var pagedNewsItems = latestNewsItems.Skip(pageNumber * pageSize).Take(pageSize);
            return pagedNewsItems;
        }

        /// <summary>
        /// Verify every news items in an id list are stored in cache, or get from API those that aren't
        /// Remove from cache items that are not in list
        /// </summary>
        /// <param name="newsItems"></param>
        /// <param name="cachedItems"></param>
        /// <returns></returns>
        private async Task<List<NewsItem>> EnsureAllItemsInCache(IEnumerable<int> newsItems, List<NewsItem>cachedItems)
        {
            
            var itemsNotInCache = newsItems.Where(ni => !cachedItems.Any(ci => ci.Id == ni));
            if (itemsNotInCache.Any())
            {
                ConcurrentBag<NewsItem> itemsResult = new ConcurrentBag<NewsItem>();
                await Parallel.ForEachAsync(itemsNotInCache, async (newsItem, cancellationToken) =>
                {
                    var item = await _serviceAgent.GetItem(newsItem);
                    if (item is not null)
                    {
                        itemsResult.Add(new NewsItem
                        {
                            Id = item.id,
                            Title = item.title,
                            Url = item.url,
                        });
                    }
                });
                cachedItems.AddRange(itemsResult);
                // remove old entries from cache
                cachedItems.RemoveAll(ci => !newsItems.Contains(ci.Id));
                cacheService.Set(Constants.NewsItemsKey, cachedItems);
            }
            return cachedItems;
        }
    }
}
