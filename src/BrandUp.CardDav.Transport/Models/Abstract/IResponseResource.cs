using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IResponseResource : IXmlSerializable
    {
        public string Endpoint { get; }
        public PropertyDictionary FoundProperties { get; }
    }

    public class DefaultResponseResource : IResponseResource
    {
        public string Endpoint { get; set; }

        public PropertyDictionary FoundProperties { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
