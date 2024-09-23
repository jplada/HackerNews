using HackerNews.Services.Interfaces;
using HackerNews.Services.Models.HackerNews;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace HackerNews.Services.Services
{
    /// <summary>
    /// Service class to query Hacker News external API
    /// </summary>
    public class HackerNewsServiceAgent : IHackerNewsServiceAgent
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        public HackerNewsServiceAgent(IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }
        public async Task<Item> GetItem(int id)
        {
            var httpRequest = new HttpRequestMessage();
            httpRequest.Headers.Add("Accept", "application/json");
            httpRequest.Method = HttpMethod.Get;
            var url = configuration.GetValue<string>("HackerNewsUrls:GetItem");
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Missing configuration HackerNewsUrls:GetItem");
            }
            httpRequest.RequestUri = new Uri(url.Replace("{itemID}", id.ToString()), UriKind.Absolute);
            var httpClient = httpClientFactory.CreateClient("HackerNewsAPI");
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponse.Content.ReadAsStreamAsync();
                if (contentStream is not null)
                {
                    StreamReader reader = new StreamReader(contentStream);
                    string jsonResponse = reader.ReadToEnd();
                    var item = JsonSerializer.Deserialize<Item>(jsonResponse);
                    return item;
                }
                throw new ArgumentException("Error accessing Hacker News API");
            }
            else
            {
                throw new ArgumentException("Error accessing Hacker News API");
            }
        }

        public async Task<IEnumerable<int>> GetLatest()
        {
            var httpRequest = new HttpRequestMessage();
            httpRequest.Headers.Add("Accept", "application/json");
            httpRequest.Method = HttpMethod.Get;
            var url = configuration.GetValue<string>("HackerNewsUrls:GetLatest");
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("Missing configuration HackerNewsUrls:GetLatest");
            }
            httpRequest.RequestUri = new Uri(url, UriKind.Absolute);
            var httpClient = httpClientFactory.CreateClient("HackerNewsAPI");
            var httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponse.Content.ReadAsStreamAsync();
                if (contentStream is not null)
                {
                    StreamReader reader = new StreamReader(contentStream);
                    string jsonResponse = reader.ReadToEnd();
                    var latestNewsIds = JsonSerializer.Deserialize<IEnumerable<int>>(jsonResponse);
                    return latestNewsIds;
                }
                throw new ArgumentException("Error accessing Hacker News API");
            }
            else
            {
                throw new ArgumentException("Error accessing Hacker News API");
            }
        }
    }
}
