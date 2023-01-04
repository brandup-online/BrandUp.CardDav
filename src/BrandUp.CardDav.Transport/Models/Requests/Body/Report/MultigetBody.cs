using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Report
{
    /// <summary>
    /// <see href="https://www.rfc-editor.org/rfc/rfc6352.html#section-10.7"/>
    /// </summary>
    [XmlRoot(ElementName = "addressbook-multiget", Namespace = "urn:ietf:params:xml:ns:carddav")]
    public class MultigetBody : IReportBody
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

        #region IVCardCondition members

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IEnumerable<T> FillterCollection<T>(IEnumerable<T> collection)
        {
            var endpoints = VCardEndpoints.Select(e => e.Split('/').Last()).ToArray();

            if (!typeof(T).IsAssignableTo(typeof(IContactDocument)))
                throw new ArgumentException("Expected IContactDocument collection");

            return collection.Cast<IContactDocument>().Where(c => endpoints.Contains(c.Name)).Cast<T>().ToArray();
        }

        #endregion

        #region IXmlSerializable members

        XmlSchema IXmlSerializable.GetSchema() => null;


        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var propList = new List<IDavProperty>();
            var endpoints = new List<string>();
            while (reader.Read())
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
                        reader.Read();
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
