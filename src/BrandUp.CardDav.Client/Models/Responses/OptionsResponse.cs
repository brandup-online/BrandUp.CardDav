namespace BrandUp.CardDav.Client.Models.Responses
{
    public class OptionsResponse
    {
        public bool IsSuccess { get; set; }
        public string StatusCode { get; set; }
        public string[] DavHeaderValue { get; set; }
        public string[] AllowHeaderValue { get; set; }
    }
}
