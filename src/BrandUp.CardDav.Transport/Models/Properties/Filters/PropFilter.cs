using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    internal class PropFilter : IFilter
    {
        #region IDavProperty members
        public string Name => "prop-filter";

        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        #endregion

        #region IFilter members

        public VCardProperty PropName { get; internal set; }
        public FilterMatchType Type { get; internal set; }

        public IEnumerable<TextMatch> Conditions { get; internal set; }

        public bool CheckConditions(VCardModel vCardModel)
        {
            bool flag = false;

            foreach (var condition in Conditions)
            {
                var values = vCardModel.GetValuesOf(PropName).Split(' ');
                foreach (var value in values)
                {
                    flag = condition.Check(value);
                    if (Type == FilterMatchType.All && flag == false)
                        return false;
                }
            }

            return flag;
        }

        #endregion

        #region IXmlSerializable members

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

        #endregion
    }
}
