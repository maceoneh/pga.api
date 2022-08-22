using System.Runtime.Serialization;
using System.Xml.Linq;

namespace pga.api.DTOs
{
    [DataContract]
    public class DTORequestGWebAPIRemoveSession
    {
        [DataMember(Name = "appkey")]
        public string ApplicationKey { get; set; }

        [DataMember(Name = "oldtoken")]
        public string OldToken { get; set; }
    }
}
