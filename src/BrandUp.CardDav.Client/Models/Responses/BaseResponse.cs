namespace BrandUp.CardDav.Client.Models.Responses
{
    public abstract class BaseContent
    {
        public string Endpoint { get; set; }
        public string Etag { get; set; }
        public string Ctag { get; set; }
    }
}
