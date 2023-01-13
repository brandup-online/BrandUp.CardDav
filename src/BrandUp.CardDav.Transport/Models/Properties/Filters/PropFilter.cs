using BrandUp.CardDav.Transport.Abstract.Enum;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters.Conditions;
using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    internal class PropFilter : IFilterProperty
    {
        #region IDavProperty members
        public string Name => "prop-filter";

        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        #endregion

        #region IFilter members

        public CardProperty PropName { get; internal set; }
        public FilterMatchType Type { get; internal set; }

        public IEnumerable<ICondition> Conditions { get; internal set; }

        public bool CheckConditions(VCardModel vCardModel)
        {
            bool flag = true;

            foreach (var condition in Conditions)
            {
                if (vCardModel.TryGetValue(PropName, out var values))
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

        async public void ReadXml(XmlReader reader)
        {
            if (reader.TryGetAttribute("name", Namespace, out var value))
                PropName = Enum.Parse<CardProperty>(value);

            if (reader.TryGetAttribute("test", Namespace, out value))
            {
                if (value == "allof")
                    Type = FilterMatchType.All;
                else
                    Type = FilterMatchType.Any;
            }

            var conditions = new List<ICondition>();
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "text-match")
                    {
                        var cond = new TextMatch() as IXmlSerializable;
                        cond.ReadXml(reader);
                        conditions.Add(cond as ICondition);
                    }
                    else if (reader.LocalName == "param-filter")
                    {
                        var cond = new ParamFilter() as IXmlSerializable;
                        cond.ReadXml(reader);
                        conditions.Add(cond as ICondition);
                    }
                    else if (reader.LocalName == "is-not-defined")
                    {
                        var cond = new IsNotDefined();
                        conditions.Add(cond);
                    }
                }
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "prop-filter")
                    break;
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
                (condition).WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
