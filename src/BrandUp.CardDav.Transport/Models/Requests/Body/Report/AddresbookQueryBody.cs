using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Report
{
    [XmlRoot(ElementName = "addressbook-query", Namespace = "urn:ietf:params:xml:ns:carddav")]
    public class AddresbookQueryBody : IReportBody
    {
        internal IEnumerable<IDavProperty> PropList { get; set; }
        internal FilterBody Filter { get; set; }

        public int Limit { get; set; } = 0;

        public AddresbookQueryBody()
        { }

        #region IRequestBody members

        public IEnumerable<IDavProperty> Properties => PropList;

        #endregion

        #region IFilter member

        public IEnumerable<T> FillterCollection<T>(IEnumerable<T> collection)
        {
            if (typeof(T).IsAssignableTo(typeof(IContactDocument)))
            {
                var contacts = collection.Cast<IContactDocument>();



                if (Limit > 0)
                    return contacts.Cast<T>().Take(Limit);
                else return contacts.Cast<T>();
            }
            else throw new ArgumentException("Expected IContactDocument collection");
        }

        #endregion

        #region IXmlSerializable member

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
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
                        var addresData = new AddressData();

                        addresData.ReadXml(reader);
                        propList.Add(addresData);
                    }
                    else if (reader.LocalName == "filter")
                    {
                        var filter = new FilterBody();

                        filter.ReadXml(reader);

                        Filter = filter;
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

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("prop", "DAV:");
            foreach (IDavProperty prop in PropList)
            {
                prop.WriteXml(writer);
            }
            writer.WriteEndElement();

            if (Filter != null)
                Filter.WriteXml(writer);

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
