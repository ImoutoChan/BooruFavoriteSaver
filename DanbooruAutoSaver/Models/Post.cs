using System.Text.Json.Serialization;

namespace DanbooruAutoSaver.Models
{
    public class Post
    {
        [JsonPropertyName("file_url")]
        public string FileUrl { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }

        [JsonIgnore]
        public bool WithoutHash { get; set; } = false;
    }
}
