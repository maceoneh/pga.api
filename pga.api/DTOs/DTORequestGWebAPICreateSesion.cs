using System.Runtime.Serialization;

namespace pga.api.DTOs
{
    [DataContract]
    public class DTORequestGWebAPICreateSesion
    {
        [DataMember(Name = "user")]
        public string User { get; set; }

        [DataMember(Name = "appkey")]
        public string ApplicationKey { get; set; }

        [DataMember(Name = "newtoken")]
        public string NewToken { get; set; }

        [DataMember(Name = "uidgroup")]
        public string UUIDGroup { get; set; }

        [DataMember(Name = "uidemploycollaborator")]
        public string UUIDEmployCollaborator { get; set; }

        [DataMember(Name = "ttl")]
        public int TTL { get; set; }
    }
}
