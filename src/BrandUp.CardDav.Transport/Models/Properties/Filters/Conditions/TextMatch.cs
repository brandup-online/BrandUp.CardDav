using BrandUp.CardDav.Transport.Abstract.Enum;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters.Conditions
{
    /// <summary>
    /// Match with value of VCard property
    /// </summary>
    public class TextMatch : ICondition
    {
        private TextCondition condition;

        /// <summary>
        /// 
        /// </summary>
        public TextMatch()
        {
            condition = new();
        }

        /// <summary>
        /// 
        /// </summary>
        public TextMatch(string text, TextMatchType matchType, bool isNegate = false, string collation = "i;unicode-casemap")
        {
            condition = new TextCondition(text, matchType, isNegate, collation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchSubstring"></param>
        /// <param name="textMatchType"></param>
        /// <param name="isNegate"></param>
        /// <returns></returns>
        public static TextMatch Create(string matchSubstring, TextMatchType textMatchType, bool isNegate = false)
        {
            return new TextMatch(matchSubstring, textMatchType, isNegate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchSubstring"></param>
        /// <param name="textMatchType"></param>
        /// <param name="collation"></param>
        /// <param name="isNegate"></param>
        /// <returns></returns>
        public static TextMatch Create(string matchSubstring, TextMatchType textMatchType, string collation, bool isNegate = false)
        {
            return new TextMatch(matchSubstring, textMatchType, isNegate, collation);
        }

        #region ICondition members

        /// <summary>
        /// Checks the condition
        /// </summary>
        /// <param name="line">VCard Value</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool Check(VCardLine line) => condition.Check(line.Value);

        #endregion

        #region IDavProperty members

        /// <summary>
        /// 
        /// </summary>
        public string Name => "text-match";

        /// <summary>
        /// 
        /// </summary>
        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        #endregion

        #region IXmlSerializable members

        XmlSchema IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            condition.ReadXml(reader);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            condition.WriteXml(writer);
        }

        #endregion
    }
}