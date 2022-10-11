using System.Text.Json.Serialization;

namespace pga.api.DTOs
{
    public class DTOResponseGWebAPIGetQueryWorkOrderByEmploy
    {        
        [JsonPropertyName("idcompany")]
        public int IdCompany { get; set; }

        [JsonPropertyName("idsubcompany")]
        public int IdSubCompany { get; set; }

        [JsonPropertyName("idrepairer")]
        public int IdRepairer { get; set; }

        [JsonPropertyName("idadministrator")]
        public int IDAdministrator { get; set; }

        [JsonPropertyName("insureddni")]
        public string InsuredDNI { get; set; }

        [JsonPropertyName("descprovider")]
        public string DescProvider { get; set; }

        [JsonPropertyName("insuredmail")]
        public string InsuredMail { get; set; }

        [JsonPropertyName("policynumber")]
        public string PolicyNumber { get; set; }

        [JsonPropertyName("claiminternalcode")]
        public string ClaimInternalCode { get; set; }

        [JsonPropertyName("claimnumber")]
        public string ClaimNumber { get; set; }

        [JsonPropertyName("assistancenumber")]
        public string AssistanceNumber { get; set; }

        [JsonPropertyName("hour")]
        public string Hour { get; set; }

        [JsonPropertyName("agreedappointment")]
        public bool AgreedAppointment { get; set; }

        [JsonPropertyName("pgamobilestatus")]
        public int PGAMobileStatus { get; set; }

        [JsonPropertyName("descguild")]
        public string DescGuild { get; set; }

        [JsonPropertyName("idmasterdetail")]
        public int IDMasterDetail { get; set; }

        [JsonPropertyName("insuredaddress")]
        public string InsuredAddress { get; set; }

        [JsonPropertyName("insuredname")]
        public string InsuredName { get; set; }

        [JsonPropertyName("insuredpopulation")]
        public string InsuredPopulation { get; set; }

        [JsonPropertyName("insuredprovince")]
        public string InsuredProvince { get; set; }

        [JsonPropertyName("insuredpostalcode")]
        public string InsuredPostalCode { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("appointmentdate")]
        public string AppointmentDate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("insuredtel1")]
        public string InsuredTel1 { get; set; }

        [JsonPropertyName("insuredtel2")]
        public string InsuredTel2 { get; set; }

        [JsonPropertyName("insuredtel3")]
        public string InsuredTel3 { get; set; }

        [JsonPropertyName("insuredtelfax")]
        public string InsuredTelFax { get; set; }

        [JsonPropertyName("idclaim")]
        public int IDClaim { get; set; }

        [JsonPropertyName("urgent_claim")]
        public bool UrgentClaim { get; set; }

        [JsonPropertyName("idfirm")]
        public int IDFirm { get; set; }

        [JsonPropertyName("idguild")]
        public int IDGuild { get; set; }

        [JsonPropertyName("descphase")]
        public string DescPhase { get; set; }
    }

    public class DTOResponseGWebAPIGetQuery
    {
        [JsonPropertyName("workorderbyemploy")]
        public List<DTOResponseGWebAPIGetQueryWorkOrderByEmploy> WorkorderByEmploy { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }
    }
}
