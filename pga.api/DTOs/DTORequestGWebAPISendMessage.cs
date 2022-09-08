using pga.core.DTOsBox;
using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTORequestGWebAPISendMessage
    {
        [JsonPropertyName("idappointment")]
        public int External_ID { get; set; }

        [JsonPropertyName("direction")]
        public EBoxMessageFlow Direction { get; set; }

        [JsonPropertyName("type")]
        public EBoxMessageType Type { get; set; }

        /// <summary>
        /// Indica si el contenido de Value viene codificado o en texto plano
        /// </summary>
        [JsonPropertyName("encode_type")]
        public EEncodeType EncodeType { get; set; }

        [JsonPropertyName("temporary_mark")]
        public long TemporaryMark { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
