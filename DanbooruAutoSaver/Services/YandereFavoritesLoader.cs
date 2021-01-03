using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using DanbooruAutoSaver.Models;

namespace DanbooruAutoSaver.Services
{
    public class YandereFavoritesLoader
    {
        private const string FavoritesUrl =
            "https://yande.re/post.json?tags=vote%3A3%3A{0}+order%3Avote&login={0}&api_key={1}";

        private readonly BooruConfiguration _booruConfiguration;
        private readonly HttpClient _httpClient;

        public YandereFavoritesLoader(HttpClient httpClient, BooruConfiguration booruConfiguration)
        {
            _httpClient = httpClient;
            _booruConfiguration = booruConfiguration;
        }

        public async IAsyncEnumerable<Post> GetFavoritesUrls()
        {
            Post[] posts;
            var page = 1;
            var url = string.Format(FavoritesUrl, _booruConfiguration.Login, _booruConfiguration.ApiKey);
            do
            {
                var result = await _httpClient.GetStringAsync(url + $"&page={page}");
                posts = JsonSerializer.Deserialize<Post[]>(result);

                foreach (var post in posts)
                {
                    post.WithoutHash = true;
                    yield return post;
                }

                page++;
            } while (posts.Any());
        }
    }
}
