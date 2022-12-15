using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    public class Filter : IDavProperty
    {
        private const string name = "filter";
        private const string @namespace = "urn:ietf:params:xml:ns:carddav";
        public IList<IFilterData> FilterData { get; set; }

        public FilterMatchType MatchType { get; set; }

        public Filter()
        {
            FilterData = new List<IFilterData>();
        }

        public Filter AddPropFilter(string propName, FilterMatchType type, params TextMatch[] conditions)
        {
            FilterData.Add(new PropFilter { PropName = propName, Type = type, Conditions = conditions });

            return this;
        }

        #region IDavProperty members

        string IDavProperty.Name => name;

        string IDavProperty.Namespace => @namespace;

        #endregion

        #region IXmlSerializable members

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(name, @namespace);
            writer.WriteAttributeString("test", MatchType.ToString().ToLowerInvariant() + "of");
            if (FilterData != null)
                foreach (var filter in FilterData)
                {
                    filter.WriteXml(writer);
                }
            writer.WriteEndElement();
        }

        #endregion
    }
}