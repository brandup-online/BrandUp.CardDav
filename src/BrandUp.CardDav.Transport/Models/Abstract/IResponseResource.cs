using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IResponseResource : IXmlSerializable
    {
        public string Endpoint { get; }
        public PropertyDictionary FoundProperties { get; }
        public IEnumerable<IDavProperty> NotFoundProperties { get; }
    }
}
