using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using DanbooruAutoSaver.Models;

namespace DanbooruAutoSaver.Services
{
    public class PostSaver
    {
        private readonly HttpClient _httpClient;

        public PostSaver(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SavePosts(IReadOnlyCollection<Post> posts, string path)
        {
            foreach (var post in posts)
            {
                var name = GetName(post);
                var response = await _httpClient.GetAsync(post.FileUrl);
                response.EnsureSuccessStatusCode();

                await using var fs = new FileStream(
                    Path.Combine(path, HttpUtility.UrlDecode(name)),
                    FileMode.CreateNew);
                await response.Content.CopyToAsync(fs);
            }
        }

        private static string GetName(Post post)
        {
            var extension = post.FileUrl.Split('/').Last().Split('.').Last();
            return $"{post.Md5}{'.'}{extension}";
        }
    }
}
