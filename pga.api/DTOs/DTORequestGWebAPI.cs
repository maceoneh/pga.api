using es.dmoreno.utils.api;
using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTORequestGWebAPI<T> : DTORequest<T>
    {
        [JsonPropertyName("aktype")]
        public string AKType { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
