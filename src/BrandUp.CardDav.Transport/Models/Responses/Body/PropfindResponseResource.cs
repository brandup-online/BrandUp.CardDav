using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    public class DefaultResponseResource : IResponseResource
    {
        public string Endpoint { get; private set; }

        public PropertyDictionary FoundProperties { get; private set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool notFoundFlag = true;
            Dictionary<IDavProperty, string> propstat = new();
            while (reader.Read())
            {
                if (reader.LocalName == "href")
                {
                    ReadHref(reader);
                    continue;
                }
                else if (reader.LocalName == "propstat")
                {
                    if (notFoundFlag)
                    {
                        propstat = new();
                    }
                    else
                    {
                        FoundProperties = new(propstat);
                        while (reader.LocalName != "response" || reader.NodeType != XmlNodeType.EndElement)
                        {
                            reader.Read();
                        }
                        return;
                    }
                }
                else if (reader.LocalName == "prop")
                {
                    ReadProp(reader, propstat);
                }
                else if (reader.LocalName == "status")
                {
                    reader.Read();
                    if (reader.Value.Contains("200"))
                        notFoundFlag = false;
                    reader.Read();
                }
                else if (reader.LocalName == "response")
                {
                    return;
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
