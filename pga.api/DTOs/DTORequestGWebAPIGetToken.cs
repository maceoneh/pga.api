using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTORequestGWebAPIGetToken
    {
        [JsonPropertyName("user")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
