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

        /// <summary>
        /// Get latest news with pagination
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PagedResponseDTO<IEnumerable<NewsItem>>> GetLatest(int pageNumber, int pageSize)
        {
            if(pageNumber<0 || pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("Page number / page size are not valid");
            }
            List<NewsItem> itemsResult = new List<NewsItem>();
            // Get Latest from external API
            var newsItems = await _serviceAgent.GetLatest();
            // Get news currently stored in cache
            var cacheNewsItems = cacheService.Get<List<NewsItem>>(Constants.NewsItemsKey);
            // Take paged latest news
            var pagedNewsIds = newsItems.Skip(pageNumber * pageSize).Take(pageSize);
            // Verify every news is in cache and get from external API those that aren't
            cacheNewsItems = await EnsureAllItemsInCache(pagedNewsIds, cacheNewsItems);
            foreach (var id in pagedNewsIds)
            {
                var newsItem = cacheNewsItems.FirstOrDefault(ni => ni.Id==id);
                if (newsItem != null)
                {
                    itemsResult.Add(newsItem);
                }                
            }
            return new PagedResponseDTO<IEnumerable<NewsItem>>
            {
                Data = itemsResult,
                CurrentPage = pageNumber,
                TotalPages = ((newsItems.Count() + pageSize - 1) / pageSize),
                Success = true
            };            
        }

        public async Task<PagedResponseDTO<IEnumerable<NewsItem>>> Search(string searchTerm, int pageNumber, int pageSize)
        {
            if (pageNumber < 0 || pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("Page number / page size are not valid");
            }            
            // Get Latest from external API
            var latestNewsItems = await _serviceAgent.GetLatest();
            // Get news currently stored in cache
            var cacheNewsItems = cacheService.Get<List<NewsItem>>(Constants.NewsItemsKey);
            // Verify every news is in cache and get from external API those that aren't
            cacheNewsItems = await EnsureAllItemsInCache(latestNewsItems, cacheNewsItems, true);
            // Set items from cache in same order as latest list
            List<NewsItem> orderedItems = new List<NewsItem>();
            foreach (var id in latestNewsItems)
            {
                var newsItem = cacheNewsItems.FirstOrDefault(ni => ni.Id == id);
                if (newsItem != null)
                {
                    orderedItems.Add(newsItem);
                }
            }
            // Search term in title
            List<NewsItem> itemsMatchSearch = orderedItems.Where(ni => ni.Title.ToLower()
                .Contains(searchTerm.ToLower())).ToList();
            // Take paged items that match search
            var pagedNewsItems = itemsMatchSearch.Skip(pageNumber * pageSize).Take(pageSize);
            return new PagedResponseDTO<IEnumerable<NewsItem>>
            {
                Data = pagedNewsItems,
                CurrentPage = pageNumber,
                TotalPages = ((itemsMatchSearch.Count() + pageSize - 1) / pageSize),
                Success = true
            };
        }

        /// <summary>
        /// Verify every news items in an id list are stored in cache, or get from API those that aren't
        /// Optionally remove from cache items that are not in list
        /// </summary>
        /// <param name="newsItems"></param>
        /// <param name="cachedItems"></param>
        /// <returns></returns>
        private async Task<List<NewsItem>> EnsureAllItemsInCache(IEnumerable<int> newsItems, List<NewsItem>cachedItems, bool removeFromCache = false)
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
                if (removeFromCache)
                {
                    // remove old entries from cache
                    cachedItems.RemoveAll(ci => !newsItems.Contains(ci.Id));
                }
                cacheService.Set(Constants.NewsItemsKey, cachedItems);
            }
            return cachedItems;
        }
    }
}
