using System.Text.Json.Serialization;

namespace DanbooruAutoSaver.Models
{
    public class RoomCheckResponse
    {
        [JsonPropertyName("md5")]
        public string Md5 { get; set; }
    }
}
