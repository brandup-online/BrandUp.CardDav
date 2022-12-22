using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IRequestBody : IXmlSerializable
    {
        IEnumerable<IDavProperty> Properties { get; }
    }
}
