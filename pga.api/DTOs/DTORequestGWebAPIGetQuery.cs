namespace pga.api.DTOs
{
    public class DTORequestGWebAPIGetQueryFilter
    { 
        public string Field { get; set; }
        public string Value { get; set; }

    }

    public class DTORequestGWebAPIGetQuery
    {
        public string Query { get; set; }
        //filters
        public bool Reduce { get; set; }
        public int Pixels { get; set; }
        public int Start { get; set; }
        public string Order { get; set; }
        public string OrderField { get; set; }
        public string ApplicationKey { get; set; }
        public bool LinkClients { get; set; }
        public bool NotLinkClaims { get; set; }
    }
}
