using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Report
{
    [XmlRoot(ElementName = "addressbook-query", Namespace = "urn:ietf:params:xml:ns:carddav")]
    public class AddresbookQueryBody : IRequestBody
    {
        internal IEnumerable<IDavProperty> PropList { get; set; }
        internal Filter Filter { get; set; }

        internal uint Limit { get; set; } = 0;

        public AddresbookQueryBody()
        { }

        #region IRequestBody members

        public IEnumerable<IDavProperty> Properties => new List<IDavProperty>(PropList) { Filter };

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

                    if (reader.LocalName == "address-data")
                    {
                        var addresData = new AddressData();

                        addresData.ReadXml(reader);
                        propList.Add(addresData);
                    }

                    if (reader.LocalName == "filter")
                    {
                        var filter = new Filter();

                        filter.ReadXml(reader);

                        Filter = filter;
                    }

                    if (reader.LocalName == "limit")
                    {
                        reader.Read();
                        Limit = uint.Parse(reader.Value);
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
