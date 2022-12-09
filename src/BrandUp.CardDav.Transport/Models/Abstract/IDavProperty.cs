using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IDavProperty : IXmlSerializable
    {
        string Name { get; }
        string Namespace { get; }
    }
}