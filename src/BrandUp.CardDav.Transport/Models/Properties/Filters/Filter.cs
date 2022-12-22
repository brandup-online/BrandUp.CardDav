using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    public class FilterBody : IDavProperty
    {
        private const string name = "filter";
        private const string @namespace = "urn:ietf:params:xml:ns:carddav";

        public IList<IFilter> Filters { get; set; }

        public FilterMatchType MatchType { get; set; }

        public FilterBody()
        {
            Filters = new List<IFilter>();
        }

        public FilterBody AddPropFilter(VCardProperty propName, FilterMatchType type, params TextMatch[] conditions)
        {
            Filters.Add(new PropFilter { PropName = propName, Type = type, Conditions = conditions });

            return this;
        }

        internal bool ApplyFilter(VCardModel vCardModel)
        {
            bool flag = false;

            foreach (var filter in Filters)
            {
                flag = filter.CheckConditions(vCardModel);
                if (MatchType == FilterMatchType.All && flag == false)
                    return false;
            }

            return flag;
        }

        #region IDavProperty members

        string IDavProperty.Name => name;

        string IDavProperty.Namespace => @namespace;

        #endregion

        #region IXmlSerializable members

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Attribute && reader.LocalName == "test")
                {
                    if (reader.Value == "allof")
                        MatchType = FilterMatchType.All;
                    else
                        MatchType = FilterMatchType.Any;
                }
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "prop-filter")
                    {
                        var propfilter = new PropFilter();

                        propfilter.ReadXml(reader);

                        Filters.Add(propfilter);
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(name, @namespace);
            writer.WriteAttributeString("test", MatchType.ToString().ToLowerInvariant() + "of");
            if (Filters != null)
                foreach (var filter in Filters)
                {
                    filter.WriteXml(writer);
                }
            writer.WriteEndElement();
        }

        #endregion
    }
}