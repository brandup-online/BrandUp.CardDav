using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    public class DefaultResponseResource : IResponseResource
    {
        public string Endpoint { get; set; }

        public PropertyDictionary FoundProperties { get; set; }

        public IEnumerable<IDavProperty> NotFoundProperties { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Dictionary<IDavProperty, string> found = new();
            List<IDavProperty> notFound = new();
            int propDepth = 0;

            while (reader.Read())
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
                        FoundProperties = new(found);
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
                    reader.Read();

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

                            reader.Read();
                        }
                    }
                    else
                    {
                        notFound.Add(prop);
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #region Xml Helpers
        private void ReadHref(XmlReader reader)
        {
            if (reader.LocalName == "href")
            {
                if (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                        Endpoint = reader.Value;
                }
                reader.Read();
            }
        }

        private void ReadProp(XmlReader reader, IDictionary<IDavProperty, string> dict)
        {
            if (reader.LocalName != "prop")
                throw new ArgumentException("Reader on incorrect line.");
            var baseDepth = reader.Depth + 1;
            DefaultProp prop = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "prop")
                    return;

                if (reader.NodeType == XmlNodeType.Element)
                    if (reader.Depth == baseDepth)
                        prop = new(reader.LocalName, reader.NamespaceURI);
                    else
                    {
                        if (prop == null)
                            throw new ArgumentNullException(nameof(prop));
                        if (reader.NodeType == XmlNodeType.Text)
                            dict.Add(prop, reader.Value);
                        else
                        {
                            if (!dict.TryAdd(prop, reader.LocalName))
                                dict[prop] = string.Join(", ", dict[prop], reader.LocalName);
                        }
                    }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    if (!dict.TryAdd(prop, reader.Value))
                        dict[prop] = reader.Value;
                }
            }
        }
        #endregion
    }
}
