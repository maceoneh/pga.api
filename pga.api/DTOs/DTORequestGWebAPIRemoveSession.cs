using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTORequestGWebAPIRemoveSession
    {
        [JsonPropertyName("appkey")]
        public string ApplicationKey { get; set; }

        [JsonPropertyName("oldtoken")]
        public string OldToken { get; set; }
    }
}
