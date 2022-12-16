using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    public class MkcolResponceResource : IResponseResource
    {
        public string Endpoint => null;

        public PropertyDictionary FoundProperties { get; private set; }

        public IEnumerable<IDavProperty> NotFoundProperties { get; private set; }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            var dict = new Dictionary<IDavProperty, string>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    dict.Add(new DefaultProp(reader.LocalName, reader.NamespaceURI), "");
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "prop")
                {
                    FoundProperties = new(dict);
                    return;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var item in FoundProperties)
            {
                writer.WriteStartElement(item.Key.Name, item.Key.Namespace);
                writer.WriteEndElement();
            }
        }
    }
}
