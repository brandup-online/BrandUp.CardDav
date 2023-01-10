using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Report
{
    /// <summary>
    /// Report query body address-book <see href="https://www.rfc-editor.org/rfc/rfc6352.html#section-10.3"/>
    /// </summary>
    [XmlRoot(ElementName = "addressbook-query", Namespace = "urn:ietf:params:xml:ns:carddav")]
    public class AddresbookQueryBody : IReportBody
    {
        internal IEnumerable<IDavProperty> PropList { get; set; }
        internal FilterBody Filter { get; set; }

        /// <summary>
        /// limit of response objects
        /// </summary>
        public int Limit { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public AddresbookQueryBody()
        { }

        #region IRequestBody members

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> Properties => PropList;

        #endregion

        #region IFilter member

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IEnumerable<T> FillterCollection<T>(IEnumerable<T> collection)
        {
            if (typeof(T).IsAssignableTo(typeof(IContactDocument)))
            {
                var contacts = collection.Cast<IContactDocument>().Where(c => Filter.ApplyFilter(new VCardModel(c.RawVCard))).ToArray();

                if (Limit > 0)
                    return contacts.Cast<T>().Take(Limit);
                else return contacts.Cast<T>();
            }
            else throw new ArgumentException("Expected IContactDocument collection");
        }

        #endregion

        #region IXmlSerializable member

        XmlSchema IXmlSerializable.GetSchema() => null;


        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var propList = new List<IDavProperty>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "prop")
                        continue;
                    else if (reader.LocalName == "address-data")
                    {
                        var addresData = (IXmlSerializable)new AddressData();

                        addresData.ReadXml(reader);
                        propList.Add((AddressData)addresData);
                    }
                    else if (reader.LocalName == "filter")
                    {
                        var filter = (IXmlSerializable)new FilterBody();

                        filter.ReadXml(reader);

                        Filter = filter as FilterBody;
                    }
                    else if (reader.LocalName == "limit")
                    {
                        reader.Read();
                        Limit = int.Parse(reader.Value);
                    }
                    else
                    {
                        propList.Add(new DefaultProp(reader.LocalName, reader.NamespaceURI));
                    }
                }
            }
            PropList = propList;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("prop", "DAV:");
            foreach (IDavProperty prop in PropList)
            {
                prop.WriteXml(writer);
            }
            writer.WriteEndElement();

            if (Filter != null)
                (Filter as IXmlSerializable).WriteXml(writer);

            if (Limit > 0)
            {
                writer.WriteStartElement("limit", "urn:ietf:params:xml:ns:carddav");
                writer.WriteStartElement("nresults", "urn:ietf:params:xml:ns:carddav");
                writer.WriteValue(Limit);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
