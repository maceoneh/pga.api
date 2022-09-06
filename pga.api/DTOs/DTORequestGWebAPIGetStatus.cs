using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTORequestGWebAPIGetStatus
    {
        [JsonPropertyName("doctype")]
        public int DocumentType { get; set; }

        [JsonPropertyName("idregistry")]
        public int IDRegistry { get; set; }
    }
}
