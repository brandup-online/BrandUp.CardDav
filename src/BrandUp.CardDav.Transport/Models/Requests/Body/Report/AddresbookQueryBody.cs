using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Report
{
    [XmlRoot(ElementName = "addressbook-query", Namespace = "urn:ietf:params:xml:ns:carddav")]
    public class AddresbookQueryBody : IRequestBody
    {
        internal IEnumerable<IDavProperty> PropList { get; set; }
        internal PropFilter Filter { get; set; }

        public AddresbookQueryBody()
        { }

        #region IRequestBody members

        public IEnumerable<IDavProperty> Properties => new List<IDavProperty>(PropList) { Filter };

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
                        var filter = new PropFilter();

                        filter.ReadXml(reader);

                        Filter = filter;
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

            Filter.WriteXml(writer);
        }

        #endregion

        #endregion
    }
}
