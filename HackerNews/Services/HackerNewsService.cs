using HackerNews.Model;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Text.Json;

namespace HackerNews.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string _baseUrl = "https://hacker-news.firebaseio.com/v0";

        public HackerNewsService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<APIResponse> GetNewStoriesAsync(int page, int pageSize, string searchTerm)
        {
            var response = await _httpClient.GetStringAsync($"{_baseUrl}/newstories.json");
            var storyIds = JsonSerializer.Deserialize<List<int>>(response);

            const string storyCacheKey = "cachedStoryList";
            List<NewsStories> cachedStories = _cache.TryGetValue(storyCacheKey, out List<NewsStories> existingCache)
                ? existingCache
                : new List<NewsStories>();

            var startIndex = (page - 1) * pageSize;
            var sourceIds = !string.IsNullOrWhiteSpace(searchTerm) ? storyIds : storyIds.Skip(startIndex).Take(pageSize).ToList();

            var List = new List<NewsStories>();

            foreach (var id in sourceIds)
            {
                //if (List.Count >= pageSize)
                //    break;

                // Check if this story is already in the cache list
                var story = cachedStories.FirstOrDefault(s => s != null && s.Id == id);

                if (story == null)// Not in cache, fetch from API
                {
                    var storyJson = await _httpClient.GetStringAsync($"{_baseUrl}/item/{id}.json");
                    story = JsonSerializer.Deserialize<NewsStories>(storyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (story != null)
                        cachedStories.Add(story); // Add to local cache list
                }

                // Apply searchTerm filter
                if (story != null && (string.IsNullOrWhiteSpace(searchTerm) || story.Title?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true))
                    List.Add(story);
            }

            // Update the story list again
            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));
            _cache.Set(storyCacheKey, cachedStories, cacheOptions);

            return new APIResponse() { Result = new { List = List.Skip(startIndex).Take(pageSize).ToList(), NoOfRecord = string.IsNullOrWhiteSpace(searchTerm) ? storyIds.Count() : List.Count() } };
        }
    }
}