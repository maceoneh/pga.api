using es.dmoreno.utils.api;
using System.Runtime.Serialization;

namespace pga.api.DTOs
{
    [DataContract]
    public class DTOResponseGWebAPI<T> : DTOResponse<T>
    {
        [DataMember(Name = "aktype")]
        public string AKType { get; set; }

        [DataMember(Name = "provider")]
        public string Provider { get; set; }

        [DataMember(Name = "request")]
        public string Request { get; set; }

        [DataMember(Name = "debug")]
        public bool Debug { get; set; }

        [DataMember(Name = "execution_time")]
        public decimal ExecutionTime { get; set; }
    }
}
