using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace pga.api.DTOs
{
    public class DTORequestGWebAPIWorkOrderState
    {
        [JsonPropertyName("idmasterdetail")]
        public int IDMasterDetail { get; set; }

        [JsonPropertyName("pgamobilestatus")]
        public int PGAMobileStatus { get; set; }
    }
}
