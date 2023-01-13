using BrandUp.CardDav.Transport.Abstract.Responces;
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
        #region IResponseBody members

        /// <summary>
        /// 
        /// </summary>
        public IList<IResponseResource> Resources { get; private set; }

        #endregion

        #region IDavProperty membersWriteStartElementAsync

        /// <summary>
        /// 
        /// </summary>
        public string Name => "mkcol-response";

        /// <summary>
        /// 
        /// </summary>
        public string Namespace => "DAV:";

        #endregion

        #region IXmlSerializable members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        XmlSchema IXmlSerializable.GetSchema() => null;

        async void IXmlSerializable.ReadXml(XmlReader reader)
        {
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "prop")
                {
                    var resource = (IXmlSerializable)new ResponseResource();
                    resource.ReadXml(reader);
                    Resources.Add(resource as IResponseResource);
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var resource in Resources)
            {
                writer.WriteStartElement("", "propstat", "DAV:");
                writer.WriteStartElement("", "prop", "DAV:");
                resource.WriteXml(writer);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

        }

        #endregion
    }
}
