using es.dmoreno.utils.api;
using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTOResponseGWebAPI<T> : DTOResponse<T>
    {
        [JsonPropertyName("aktype")]
        public string AKType { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [JsonPropertyName("request")]
        public string Request { get; set; }

        [JsonPropertyName("debug")]
        public bool Debug { get; set; }

        [JsonPropertyName("execution_time")]
        public double ExecutionTime { get; set; }
    }
}
