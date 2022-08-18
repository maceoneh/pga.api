using System.Runtime.Serialization;

namespace pga.api.DTOs
{
    [DataContract]
    public class DTORequestGWebAPIGetToken
    {
        [DataMember(Name = "user")]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}
