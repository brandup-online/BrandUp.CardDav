using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Body;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses
{

    public class PropfindResponse : IResponse
    {
        public bool IsSuccess { get; init; }
        public string StatusCode { get; init; }

        public PropdindResponseBody Body { get; init; }

        IResponseBody IResponse.Body => Body;

        public static PropfindResponse Create(HttpResponseMessage message)
        {
            var serializer = new XmlSerializer(typeof(PropdindResponseBody));

            var body = (PropdindResponseBody)serializer.Deserialize(message.Content.ReadAsStream());
            var response = new PropfindResponse { IsSuccess = message.IsSuccessStatusCode, StatusCode = message.StatusCode.ToString(), Body = body };

            return response;
        }
    }

    [XmlRoot(Namespace = "DAV:", ElementName = "multistatus")]
    public class PropdindResponseBody : IResponseBody
    {
        #region IResponseBody

        public IList<IResponseResource> Resources { get; private set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var resourseList = new List<PropfindResponseResource>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var resourse = new PropfindResponseResource();
                    resourse.ReadXml(reader);
                    resourseList.Add(resourse);
                }
            }
            Resources = resourseList.ToArray();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class PropfindResponseResource : IResponseResource
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
            Prop prop = null;
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
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.EndElement)
                            dict.Add(prop, reader.Value);
                        else
                        {
                            if (!dict.TryAdd(prop, reader.LocalName))
                                dict[prop] = string.Join(", ", dict[prop], reader.LocalName);
                        }
                    }
                else if (reader.NodeType == XmlNodeType.Text)
                {
                    dict.Add(prop, reader.Value);
                }
            }
        }
        #endregion
    }


}

