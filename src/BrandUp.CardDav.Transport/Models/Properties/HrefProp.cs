using BrandUp.CardDav.Transport.Abstract.Properties;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    internal class HrefProp : IDavProperty
    {
        internal HrefProp(string name, string @namespace = "DAV:")
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        }

        #region IDavProperty members

        public string Name { get; private set; }

        public string Namespace { get; private set; }

        public void WriteXmlWithValue(XmlWriter writer, string value)
        {
            writer.WriteStartElement(Name, Namespace);
            writer.WriteElementString("href", Namespace, value);
            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable members 

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Name = reader.LocalName;
            Namespace = reader.NamespaceURI;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(Name, Namespace, "");
        }

        #endregion
    }
}
