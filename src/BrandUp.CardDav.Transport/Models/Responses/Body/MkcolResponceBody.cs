using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    [XmlRoot(ElementName = "mkcol-response", Namespace = "DAV:")]
    public class MkcolResponceBody : IResponseBody
    {
        public IList<IResponseResource> Resources { get; private set; }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "prop")
                {
                    var resource = new MkcolResponceResource();
                    resource.ReadXml(reader);
                    Resources.Add(resource);
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var resource in Resources)
            {
                writer.WriteStartElement("propstat", "DAV:");
                writer.WriteStartElement("prop", "DAV:");
                resource.WriteXml(writer);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

        }
    }
}
