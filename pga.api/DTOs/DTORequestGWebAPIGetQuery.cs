using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTORequestGWebAPIGetQueryFilter
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class DTORequestGWebAPIGetQuery
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("fields")]
        public List<string> Fields { get; set; }

        [JsonPropertyName("filters")]
        public List<DTORequestGWebAPIGetQueryFilter> Filters { get; set; }

        [JsonPropertyName("reduce")]
        public bool Reduce { get; set; }

        [JsonPropertyName("px")]
        public int Pixels { get; set; }

        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("order")]
        public string Order { get; set; }

        [JsonPropertyName("orderfield")]
        public string OrderField { get; set; }

        [JsonPropertyName("applicationkey")]
        public string ApplicationKey { get; set; }

        [JsonPropertyName("linkclients")]
        public bool LinkClients { get; set; }

        [JsonPropertyName("notlinkclaims")]
        public bool NotLinkClaims { get; set; }
    }
}
