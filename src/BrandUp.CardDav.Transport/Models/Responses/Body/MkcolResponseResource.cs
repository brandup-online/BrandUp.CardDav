using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    /// <summary>
    /// 
    /// </summary>
    public class MkcolResponseResource : IResponseResource
    {
        /// <summary>
        /// 
        /// </summary>
        public string Endpoint => null;

        /// <summary>
        /// 
        /// </summary>
        public PropertyDictionary FoundProperties { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> NotFoundProperties { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
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

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var item in FoundProperties)
            {
                writer.WriteStartElement(item.Key.Name, item.Key.Namespace);
                writer.WriteEndElement();
            }
        }
    }
}
