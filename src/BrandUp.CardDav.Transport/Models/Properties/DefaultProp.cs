using BrandUp.CardDav.Transport.Abstract.Properties;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    internal class DefaultProp : IDavProperty
    {
        private string name;
        private string @namespace;

        string IDavProperty.Name => name;
        string IDavProperty.Namespace => @namespace;

        internal DefaultProp(string name, string @namespace = "DAV:")
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        }

        #region IDavProperty members

        public string Name => name;

        public string Namespace => @namespace;


        public void WriteXmlWithValue(XmlWriter writer, string value)
        {
            writer.WriteElementString(Name, Namespace, value);
        }

        #endregion

        #region IXmlSerializable

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            name = reader.LocalName;
            @namespace = reader.NamespaceURI;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(Name, Namespace, "");
        }

        #endregion
    }
}
