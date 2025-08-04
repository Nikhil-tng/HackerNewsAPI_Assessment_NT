using HackerNews.Model;

namespace HackerNews.Services
{
    public interface IHackerNewsService
    {
        Task<APIResponse> GetNewStoriesAsync(int page, int pageSize, string searchTerm);
    }
}