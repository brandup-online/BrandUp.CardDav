using BrandUp.CardDav.Transport.Models.Responses;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IResponseResource
    {
        public string Endpoint { get; }
        public PropertyDictionary FoundProperties { get; }
    }

    public class DefaultResponseResource : IResponseResource
    {
        public string Endpoint { get; set; }

        public PropertyDictionary FoundProperties { get; set; }
    }
}
