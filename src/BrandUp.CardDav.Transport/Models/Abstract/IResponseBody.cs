using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IResponseBody : IXmlSerializable
    {
        IList<IResponseResource> Resources { get; }
    }
}
