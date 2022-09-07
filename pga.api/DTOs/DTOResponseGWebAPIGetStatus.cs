using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTOResponseGWebAPIGetStatus
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }
    }
}
