using Newtonsoft.Json;
using VirtusaTest.Models;

namespace VirtusaTest.Services
{
    public class NewsService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0/";

        public NewsService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IEnumerable<int>> GetBestStoryIdsAsync()
        {
            var response = await _httpClient.GetStringAsync($"{BaseUrl}beststories.json");
            return JsonConvert.DeserializeObject<IEnumerable<int>>(response);
        }

        public async Task<StoryModel> GetStoryDetailsAsync(int storyId)
        {
            var response = await _httpClient.GetStringAsync($"{BaseUrl}item/{storyId}.json");
            return JsonConvert.DeserializeObject<StoryModel>(response);
        }
    }
}
