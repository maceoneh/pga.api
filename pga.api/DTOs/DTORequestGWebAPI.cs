using es.dmoreno.utils.api;
using System.Runtime.Serialization;

namespace pga.api.DTOs
{
    [DataContract]
    public class DTORequestGWebAPI<T> : DTORequest<T>
    {
        [DataMember(Name = "aktype")]
        public string AKType { get; set; }

        [DataMember(Name = "provider")]
        public string Provider { get; set; }

        [DataMember(Name = "action")]
        public string Action { get; set; }
    }
}
