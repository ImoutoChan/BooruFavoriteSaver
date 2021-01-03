using System.Text.Json.Serialization;

namespace DanbooruAutoSaver.Models
{
    public class RoomCheckRequest
    {
        [JsonPropertyName("md5")]
        public string[] Md5Hashes { get; set; }
    }
}
