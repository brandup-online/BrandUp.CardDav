using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Requests;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Mkcol
{
    /// <summary>
    /// 
    /// </summary>
    public class SetPropBody : IRequestBody
    {
        private string name;
        private string description;

        /// <summary>
        /// 
        /// </summary>
        public SetPropBody()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SetPropBody(string name, string description)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.description = description ?? throw new ArgumentNullException(nameof(description));
        }

        #region IRequestBody members 

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> Properties { get; }

        string IRequestBody.Name => "set";

        string IRequestBody.Namespace => "DAV:";

        #endregion

        #region IXmlSerializable members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        async void IXmlSerializable.ReadXml(XmlReader reader)
        {
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "displayname")
                {
                    await reader.ReadAsync();
                    name = reader.Value;
                }

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "addressbook-description")
                {
                    while (reader.NodeType != XmlNodeType.Text)
                    {
                        await reader.ReadAsync();
                    }
                    description = reader.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
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
