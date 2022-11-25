namespace BrandUp.Carddav.Client.Models.Responses
{
    public abstract class BaseResponse
    {
        public string Endpoint { get; set; }
        public string Etag { get; set; }
    }
}
