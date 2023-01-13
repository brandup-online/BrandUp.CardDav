using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    /// <summary>
    /// 
    /// </summary>
    public class ResponseResource : IResponseResource
    {
        /// <summary>
        /// 
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<IDavProperty, string> FoundProperties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> NotFoundProperties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        async void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Dictionary<IDavProperty, string> found = new();
            List<IDavProperty> notFound = new();
            int propDepth = 0;

            while (await reader.ReadAsync())
            {
                if (reader.LocalName == "href")
                {
                    ReadHref(reader);
                    continue;
                }

                if (reader.LocalName == "propstat")
                    continue;

                if (reader.LocalName == "response")
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        continue;
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        FoundProperties = new PropertyDictionary(found);
                        NotFoundProperties = notFound;

                        return;
                    }
                }

                if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "prop")
                {
                    propDepth = reader.Depth;
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Depth > propDepth && propDepth != 0)
                {
                    var prop = new DefaultProp(reader.LocalName, reader.NamespaceURI);
                    await reader.ReadAsync();

                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        found.TryAdd(prop, reader.Value);
                        continue;
                    }

                    if (reader.Depth > propDepth + 1)
                    {
                        while (reader.Depth > propDepth + 1)
                        {
                            if (!found.TryAdd(prop, reader.LocalName))
                                found[prop] = string.Join(", ", found[prop], reader.LocalName);

                            await reader.ReadAsync();
                        }
                    }
                    else
                    {
                        notFound.Add(prop);
                    }
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {

            writer.WriteStartElement("", "response", "DAV:");

            writer.WriteStartElement("", "href", "DAV:");
            writer.WriteString(Endpoint);
            writer.WriteEndElement();

            if (FoundProperties.Any())
            {
                writer.WriteStartElement("", "propstat", "DAV:");

                writer.WriteStartElement("", "prop", "DAV:");
                foreach (var prop in FoundProperties)
                {
                    writer.WriteElementString("", prop.Key.Name, prop.Key.Namespace, prop.Value);
                }
                writer.WriteEndElement();

                writer.WriteElementString("", "status", "DAV:", "HTTP/1.1 200 OK");

                writer.WriteEndElement();
            }

            if (NotFoundProperties.Any())
            {

                writer.WriteStartElement("", "propstat", "DAV:");

                writer.WriteStartElement("", "prop", "DAV:");
                foreach (var prop in NotFoundProperties)
                {
                    prop.WriteXml(writer);
                }
                writer.WriteEndElement();

                writer.WriteElementString("", "status", "DAV:", "HTTP/1.1 404 Not Found");

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #region Xml Helpers

        async private void ReadHref(XmlReader reader)
        {
            if (reader.LocalName == "href")
            {
                if (await reader.ReadAsync())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                        Endpoint = reader.Value;
                }
                await reader.ReadAsync();
            }
        }

        #endregion
    }
}
