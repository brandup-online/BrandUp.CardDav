using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    internal class PropFilter : IFilterData
    {
        public VCardProperty PropName { get; internal set; }
        public FilterMatchType Type { get; internal set; }

        public IEnumerable<TextMatch> Conditions { get; internal set; }

        public string Name => "prop-filter";

        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            var conditions = new List<TextMatch>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Attribute)
                {
                    if (reader.LocalName == "name")
                    {
                        PropName = Enum.Parse<VCardProperty>(reader.Value);
                    }

                    if (reader.LocalName == "test")
                    {
                        if (reader.Value == "allof")
                            Type = FilterMatchType.All;
                        else
                        {
                            Type = FilterMatchType.Any;
                        };
                    }
                }

                if (reader.NodeType == XmlNodeType.Element)
                    if (reader.LocalName == "text-match")
                    {
                        var cond = new TextMatch();
                        cond.ReadXml(reader);
                        conditions.Add(cond);
                    }
            }
            Conditions = conditions;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            writer.WriteAttributeString("name", PropName.ToString());
            writer.WriteAttributeString("test", Type.ToString().ToLowerInvariant() + "of");
            foreach (var condition in Conditions)
            {
                condition.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
    }
}
