using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Body;
using System.Xml;

namespace BrandUp.CardDav.Transport
{
    internal class XmlSerializer
    {
        public XmlSerializer() { }

        public static string Serialize(IXmlConvertMetadata metadata)
        {
            var doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);

            //create the root element
            XmlElement root = (XmlElement)doc.AppendChild(doc.CreateElement(metadata.Name, metadata.Namespace));
            doc.InsertBefore(xmlDeclaration, root);

            GenerateNode(doc, root, metadata.Inner);

            return doc.OuterXml;
        }

        public static IEnumerable<IResponseResource> DeserializeToResourses(Stream contentStream, bool closeStream = true)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(contentStream);

            if (closeStream)
                contentStream.Dispose();

            var resources = new List<IResponseResource>();


            foreach (XmlNode responseNode in xmlDocument["multistatus", "DAV:"].ChildNodes)
            {
                var properties = new Dictionary<IDavProperty, string>();
                var resource = new DefaultResponseResource();

                if (responseNode.ChildNodes != null)
                    foreach (XmlNode propstatNode in responseNode.ChildNodes)
                    {
                        if (propstatNode.LocalName == "href")
                        {
                            resource.Endpoint = propstatNode.InnerText;
                        }
                        else if (propstatNode.LocalName == "propstat")
                        {
                            if (propstatNode["status", "DAV:"] == null)
                                continue;

                            if (propstatNode["status", "DAV:"].InnerText.Contains(" 2"))
                            {
                                var propNode = propstatNode["prop", "DAV:"];
                                if (propNode == null)
                                    break;
                                else
                                {
                                    foreach (XmlNode prop in propNode.ChildNodes)
                                    {
                                        if (prop.InnerText != string.Empty)
                                            properties.Add(new Prop(prop.LocalName, prop.NamespaceURI), prop.InnerText);
                                        else
                                        {
                                            var values = "";
                                            foreach (XmlNode valuesProp in prop.ChildNodes)
                                            {
                                                values = string.Join(" ", values, valuesProp.LocalName);
                                            }

                                            properties.Add(new Prop(prop.LocalName, prop.NamespaceURI), values);
                                        }

                                    }
                                    break;
                                }
                            }
                        }
                    }

                resource.FoundProperties = new(properties);
                resources.Add(resource);
            }

            return resources;
        }

        #region Helpers

        static void GenerateNode(XmlDocument doc, XmlNode node, IEnumerable<IXmlConvertMetadata> metadataList)
        {
            foreach (var metadata in metadataList)
            {
                var newNode = node.AppendChild(doc.CreateElement(metadata.Name, metadata.Namespace));
                if (metadata.Inner != null)
                {
                    GenerateNode(doc, newNode, metadata.Inner);
                }
            }
        }

        #endregion
    }
}
