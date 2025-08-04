using HackerNews.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace HackerNews.UnitTests;

public class HackerNewsServiceTests
{
    [Fact]
    public async Task GetNewestStoriesAsync_ReturnsStoriesFromApi()
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
        var story = new { id = 1, title = "Test Story", url = "http://test.com", score = 100, by = "testuser", time = 1234567890L };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
            {
                if (request.RequestUri.ToString().Contains("newstories"))
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(JsonSerializer.Serialize(storyIds))
                    };
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(story))
                };
            });

        // Act
        var result = await _service.GetNewStoriesAsync(1, 2, "");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Story", result[0].Title);
    }
}