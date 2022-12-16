using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Mkcol
{
    public class SetPropBody : IRequestBody
    {
        private string name;
        private string description;

        public SetPropBody()
        {

        }

        public SetPropBody(string name, string description)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.description = description ?? throw new ArgumentNullException(nameof(description));
        }

        #region IRequestBody members 

        public IEnumerable<IDavProperty> Properties { get; }

        #endregion

        #region IXmlSerializable members

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "displayname")
                {
                    reader.Read();
                    name = reader.Value;
                }

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "addressbook-description")
                {
                    while (reader.NodeType != XmlNodeType.Text)
                    {
                        reader.Read();
                    }
                    description = reader.Value;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("set", "DAV:");
            writer.WriteStartElement("prop", "DAV:");

            writer.WriteStartElement("resourcetype", "DAV:");
            writer.WriteElementString("collection", "DAV:", "");
            writer.WriteElementString("addressbook", "DAV:", "");
            writer.WriteEndElement();

            writer.WriteElementString("displayname", "DAV:", name);
            writer.WriteElementString("addressbook-description", "urn:ietf:params:xml:ns:carddav", description);

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        #endregion
    }
}
