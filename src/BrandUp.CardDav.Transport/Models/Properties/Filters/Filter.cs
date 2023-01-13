using BrandUp.CardDav.Transport.Abstract.Enum;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class FilterBody : IDavProperty
    {
        private const string name = "filter";
        private const string @namespace = "urn:ietf:params:xml:ns:carddav";

        /// <summary>
        /// 
        /// </summary>
        public IList<IFilterProperty> Filters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FilterMatchType MatchType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FilterBody()
        {
            Filters = new List<IFilterProperty>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="type"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public FilterBody AddPropFilter(CardProperty propName, FilterMatchType type, params ICondition[] conditions)
        {
            Filters.Add(new PropFilter { PropName = propName, Type = type, Conditions = conditions });

            return this;
        }

        internal bool ApplyFilter(VCardModel vCardModel)
        {
            bool flag = true;

            foreach (var filter in Filters)
            {
                flag = filter.CheckConditions(vCardModel);
                if (MatchType == FilterMatchType.All && flag == false)
                    return false;
                if (MatchType == FilterMatchType.Any && flag == true)
                    return true;
            }

            return flag;
        }

        #region IDavProperty members

        string IDavProperty.Name => name;

        string IDavProperty.Namespace => @namespace;

        #endregion

        #region IXmlSerializable members

        XmlSchema IXmlSerializable.GetSchema() => null;

        async void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.TryGetAttribute("test", @namespace, out var value))
            {
                if (value == "allof")
                    MatchType = FilterMatchType.All;
                else
                    MatchType = FilterMatchType.Any;
            }

            while (await reader.ReadAsync())
            {
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

        void IXmlSerializable.WriteXml(XmlWriter writer)
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