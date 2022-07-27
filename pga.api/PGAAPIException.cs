namespace pga.api
{
    public class PGAAPIException : Exception
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
}
