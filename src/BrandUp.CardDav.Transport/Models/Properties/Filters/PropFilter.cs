using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    internal class PropFilter : IFilterData
    {
        public string PropName { get; set; }
        public FilterMatchType Type { get; set; }

        public IEnumerable<TextMatch> Conditions { get; init; }

        public string Name => "prop-filter";

        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            writer.WriteAttributeString("name", PropName);
            writer.WriteAttributeString("test", Type.ToString().ToLowerInvariant() + "of");
            foreach (var condition in Conditions)
            {
                condition.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}
