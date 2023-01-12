using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters.Conditions
{
    /// <summary>
    /// 
    /// </summary>
    public class ParamFilter : ICondition
    {
        private CardParameter name;
        private bool isDefine = true;
        private TextCondition condition;

        internal ParamFilter()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="condition"></param>
        /// <param name="isDefined"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal ParamFilter(CardParameter name, TextCondition condition, bool isDefined = true)
        {
            this.name = name;
            this.condition = condition;
            this.isDefine = isDefined;
        }

        #region Static constructors 

        /// <summary>
        /// Creates instance of ParamFilter that will check existence of parameter in vcard line.
        /// </summary>
        /// <param name="name">Name of parameter</param>
        /// <returns></returns>
        public static ParamFilter NotDefined(CardParameter name)
        {
            return new(name, null, false);
        }

        /// <summary>
        /// Creates instance of ParamFilter that will match parameter value in vcard line with text.
        /// </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="text">Match text.</param>
        /// <param name="matchType">Match type <see cref="TextMatchType"/></param>
        /// <param name="isNegate">Negate condition.</param>
        /// <returns></returns>
        public static ParamFilter TextMatch(CardParameter name, string text, TextMatchType matchType, bool isNegate = false)
        {
            return new(name, new(text, matchType, isNegate));
        }

        #endregion

        #region ICondition member

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool Check(VCardLine line)
        {
            var matched = line.Parameters.Where(_ => _.Parameter == name);

            if (!isDefine)
                return !matched.Any();

            foreach (var parameter in matched)
            {
                if (condition.Check(parameter.Value))
                    return true;
            }


            return false;
        }

        #endregion

        #region IXmlSerializable member

        XmlSchema IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.TryGetAttribute("name", Namespace, out var value))
                name = Enum.Parse<CardParameter>(value);
            else throw new ArgumentException();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "text-match")
                    {
                        var cond = new TextCondition();
                        cond.ReadXml(reader);
                        condition = cond;
                    }
                    else if (reader.LocalName == "is-not-defined")
                    {
                        isDefine = false;
                    }
                }
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "param-filter")
                    break;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            writer.WriteAttributeString("name", name.ToString().ToUpperInvariant());
            if (!isDefine)
            {
                writer.WriteStartElement("is-not-defined", "urn:ietf:params:xml:ns:carddav");
                writer.WriteEndElement();
            }
            else if (condition != null)
            {
                condition.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region IDavProperty member

        /// <summary>
        /// Xml property name
        /// </summary>
        public string Name => "param-filter";

        /// <summary>
        /// Xml property namespace
        /// </summary>
        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        #endregion
    }
}
