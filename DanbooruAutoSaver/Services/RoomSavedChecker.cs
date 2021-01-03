using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DanbooruAutoSaver.Models;

namespace DanbooruAutoSaver.Services
{
    public class RoomSavedChecker
    {
        private const string RoomCheckUrl = "http://miyu:11301/api/CollectionFiles";
        private readonly HttpClient _httpClient;

        public RoomSavedChecker(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<Post>> GetOnlyNewPosts(IReadOnlyCollection<Post> allPosts)
        {
            var request = new RoomCheckRequest
            {
                Md5Hashes = allPosts.Select(x => x.Md5).ToArray()
            };

            var results = await _httpClient.PostAsync(
                RoomCheckUrl,
                new StringContent(JsonSerializer.Serialize(request), Encoding.Unicode, "application/json"));
            results.EnsureSuccessStatusCode();

            var savedResult = await results.Content.ReadAsStringAsync();

            var existed = (JsonSerializer.Deserialize<RoomCheckResponse[]>(savedResult)
                           ?? Array.Empty<RoomCheckResponse>())
                .Select(x => x.Md5)
                .ToHashSet();

            return allPosts.Where(x => !existed.Contains(x.Md5)).ToArray();
        }
    }
}
