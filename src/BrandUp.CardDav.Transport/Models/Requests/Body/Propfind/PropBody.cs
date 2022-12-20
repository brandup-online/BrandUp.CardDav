using BrandUp.CardDav.Transport.Attributes;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Propfind
{
    [Propfind]
    public class PropBody : IRequestBody
    {
        private string name;
        private string @namespace;

        #region IRequestBody members

        public IEnumerable<IDavProperty> Properties { get; set; }

        #endregion

        public string Name => name;

        public string Namespace => @namespace;

        public PropBody()
        {

        }

        public PropBody(string name, string @namespace = "DAV:")
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        }

        #region IXmlSerializable region

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            name = reader.LocalName;
            @namespace = reader.NamespaceURI;
            var props = new List<IDavProperty>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName != "prop")
                    props.Add(new DefaultProp(reader.LocalName, reader.NamespaceURI));
            }
            Properties = props;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            if (Properties != null)
                foreach (IDavProperty property in Properties)
                {
                    property.WriteXml(writer);
                }
            writer.WriteEndElement();
        }

        #endregion
    }
}
