using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(ElementName = "mkcol-response", Namespace = "DAV:")]
    public class MkcolResponseBody : IResponseBody
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<IResponseResource> Resources { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "prop")
                {
                    var resource = (IXmlSerializable)new MkcolResponseResource();
                    resource.ReadXml(reader);
                    Resources.Add(resource as IResponseResource);
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
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
