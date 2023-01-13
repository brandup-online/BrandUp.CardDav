using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Helpers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Report
{
    /// <summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc6352.html#section-10.7"/>
    /// </summary>
    [XmlRoot(ElementName = "addressbook-multiget", Namespace = "urn:ietf:params:xml:ns:carddav")]
    public class MultigetBody : IRequestBody, IResponseCreator
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> PropList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> VCardEndpoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MultigetBody()
        { }

        #region IRequestBody members

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> Properties => PropList;


        #endregion

        #region IResponseCreator members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public IResponseBody CreateResponse(IDictionary<string, IDavDocument> collection)
        {
            var response = new MultistatusResponseBody();

            var filtered = FillterCollection(collection);

            foreach (var pair in filtered)
            {

                (var found, var notFound) = ResponseResourseHelper.GeneratePropfindResource(pair.Value, Properties);

                response.Resources.Add(new ResponseResource() { Endpoint = pair.Key, FoundProperties = found, NotFoundProperties = notFound });
            }

            return response;
        }

        #endregion

        #region IVCardCondition members

        IDictionary<string, IDavDocument> FillterCollection(IDictionary<string, IDavDocument> collection)
        {
            var endpoints = VCardEndpoints.Select(e => e.Split('/').Last()).ToArray();

            return collection.Where(c => endpoints.Contains(c.Value.Name)).ToDictionary(k => k.Key, v => v.Value);
        }

        #endregion

        #region IDavProperty members

        /// <summary>
        /// 
        /// </summary>
        public string Name => "addressbook-multiget";

        /// <summary>
        /// 
        /// </summary>
        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        #endregion

        #region IXmlSerializable members

        XmlSchema IXmlSerializable.GetSchema() => null;


        async void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var propList = new List<IDavProperty>();
            var endpoints = new List<string>();
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "prop")
                        continue;

                    if (reader.LocalName == "address-data")
                    {
                        var addresData = (IXmlSerializable)new AddressData();

                        addresData.ReadXml(reader);
                        propList.Add((AddressData)addresData);
                    }
                    else if (reader.LocalName == "href")
                    {
                        await reader.ReadAsync();
                        endpoints.Add(reader.Value);
                    }
                    else
                    {
                        propList.Add(new DefaultProp(reader.LocalName, reader.NamespaceURI));
                    }
                }
            }
            PropList = propList;
            VCardEndpoints = endpoints;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("prop", "urn:ietf:params:xml:ns:carddav");
            foreach (IDavProperty p in PropList)
            {
                p.WriteXml(writer);
            }
            writer.WriteEndElement();

            foreach (string href in VCardEndpoints)
            {
                writer.WriteElementString("href", "DAV:", href);
            }
        }

        #endregion
    }
}
