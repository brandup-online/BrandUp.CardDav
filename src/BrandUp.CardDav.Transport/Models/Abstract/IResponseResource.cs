namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IResponseResource
    {
        public string Endpoint { get; }
        public IReadOnlyDictionary<IDavProperty, string> FoundProperties { get; }
    }

    public class DefaultResponseResource : IResponseResource
    {
        public string Endpoint { get; set; }

        public IReadOnlyDictionary<IDavProperty, string> FoundProperties { get; set; }
    }
}
