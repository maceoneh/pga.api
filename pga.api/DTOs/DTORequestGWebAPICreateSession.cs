using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTORequestGWebAPICreateSession
    {
        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("appkey")]
        public string ApplicationKey { get; set; }

        [JsonPropertyName("newtoken")]
        public string NewToken { get; set; }

        [JsonPropertyName("uidgroup")]
        public string UUIDGroup { get; set; }

        [JsonPropertyName("uidemploycollaborator")]
        public string UUIDEmployCollaborator { get; set; }

        [JsonPropertyName("ttl")]
        public int TTL { get; set; }
    }
}
