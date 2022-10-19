using es.dmoreno.utils.dataaccess.db;
using es.dmoreno.utils.dataaccess.filters;
using pga.core.DTOsBox;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace pga.api.DTOs
{

    public class DTORequestCreateAppointment
    {
        [JsonPropertyName("userPGAMobile")]
        public string PGAMobileUser { get; set; }

        //Datos de la cita/parte
        [JsonPropertyName("dateFrom")]
        public DateTime DateFrom { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("guild")]
        public string? Guild { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("postalCode")]        
        public string? PostalCode { get; set; }

        [JsonPropertyName("province")]        
        public string? Province { get; set; }

        [JsonPropertyName("population")]
        public string? Population { get; set; }

        [JsonPropertyName("contactPerson1")]
        public string? ContactPerson1 { get; set; }

        [JsonPropertyName("phoneContact1")]
        public string? PhoneContact1 { get; set; }

        [JsonPropertyName("contactPerson2")]
        public string? ContactPerson2 { get; set; }

        [JsonPropertyName("phoneContact2")]
        public string? PhoneContact2 { get; set; }

        [JsonPropertyName("contactPerson3")]
        public string? ContactPerson3 { get; set; }

        [JsonPropertyName("phoneContact3")]
        public string? PhoneContact3 { get; set; }

        [JsonPropertyName("externalID")]
        public string? ExternalID { get; set; }

        //Datos del sujeto de la intervencion
        [JsonPropertyName("receiver")]
        public DTOBoxSubject Receiver { get; set; }

        //Datos del expediente
        [JsonPropertyName("provider")]        
        public DTOBoxSubject? Provider { get; set; }

        [JsonPropertyName("intermediary")]
        public DTOBoxSubject? Intermediary { get; set; }

        [JsonPropertyName("policy")]
        public string? Policy { get; set; }

        [JsonPropertyName("externalIDFile")]
        public string? ExternalIDFile { get; set; }
    }
}
