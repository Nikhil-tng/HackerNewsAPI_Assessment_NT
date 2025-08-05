using HackerNews.Model;
using HackerNews.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using Xunit;

namespace HackerNews.UnitTests
{
    public class HackerNewsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly HackerNewsService _service;

        public HackerNewsServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _service = new HackerNewsService(_httpClient, _memoryCache);
        }

        [Fact]
        public async Task GetNewStoriesAsync_ReturnsPagedStories()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3 };

            var stories = new Dictionary<int, NewsStories>
            {
                { 1, new NewsStories { Id = 1, Title = "Story One", Url = "http://story1.com", Score = 100, By = "user1", Time = 1234567890, Type = "story", Descendants = 5 } },
                { 2, new NewsStories { Id = 2, Title = "Story Two", Url = "http://story2.com", Score = 90, By = "user2", Time = 1234567891, Type = "story", Descendants = 3 } },
                { 3, new NewsStories { Id = 3, Title = "Story Three", Url = "http://story3.com", Score = 80, By = "user3", Time = 1234567892, Type = "story", Descendants = 2 } }
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    var url = request.RequestUri.ToString();

                    if (url.Contains("newstories"))
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(JsonSerializer.Serialize(storyIds))
                        };
                    }
                    else
                    {
                        var id = int.Parse(url.Split('/').Last().Split('.').First());
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(JsonSerializer.Serialize(stories[id]))
                        };
                    }
                });

            // Act
            var response = await _service.GetNewStoriesAsync(1, 1, ""); // pageSize = 1 to return one story

            // Assert
            Assert.NotNull(response);
            Assert.Equal(1, response.Status);

            // Parse dynamic response.Result into a concrete type
            var json = JsonSerializer.Serialize(response.Result);
            var resultDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            var storyList = JsonSerializer.Deserialize<List<NewsStories>>(resultDict["List"].GetRawText());
            int count = resultDict["NoOfRecord"].GetInt32();

            Assert.NotNull(storyList);
            Assert.Single(storyList);
            Assert.Equal("Story One", storyList[0].Title);
            Assert.Equal(3, count); // total number of stories
        }
    }
}
