using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    /// <summary>
    /// Match with value of VCard property
    /// </summary>
    public class TextMatch : ICondition
    {
        /// <summary>
        /// Condition value
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Collation { get; set; } = "i;unicode-casemap";

        /// <summary>
        /// Condition type
        /// </summary>
        public TextMatchType MatchType { get; set; }

        /// <summary>
        /// Flag: true - condition is invert, false - is positive
        /// </summary>
        public bool IsNegate { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public TextMatch() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matchSubstring"></param>
        /// <param name="textMatchType"></param>
        /// <param name="isNegate"></param>
        /// <returns></returns>
        public static TextMatch Create(string matchSubstring, TextMatchType textMatchType, bool isNegate = false)
        {
            return new TextMatch() { Text = matchSubstring, IsNegate = isNegate, MatchType = textMatchType };
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
            return new TextMatch() { Text = matchSubstring, IsNegate = isNegate, MatchType = textMatchType, Collation = collation };
        }

        #region ICondition members

        /// <summary>
        /// Checks the condition
        /// </summary>
        /// <param name="value">VCard Value</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool Check(string value)
        {
            switch (MatchType)
            {
                case TextMatchType.Equals: return IsNegate ^ value.Equals(Text);
                case TextMatchType.Contains: return IsNegate ^ value.Contains(Text);
                case TextMatchType.StartsWith: return IsNegate ^ value.StartsWith(Text);
                case TextMatchType.EndsWith: return IsNegate ^ value.EndsWith(Text);
                default: throw new ArgumentException("Unexpected value of enum");
            }
        }
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
            if (reader.TryGetAttribute("collation", Namespace, out var value))
                Collation = value;

            if (reader.TryGetAttribute("match-type", Namespace, out value))
                MatchType = Enum.Parse<TextMatchType>(FromKebabCase(value), true);

            if (reader.TryGetAttribute("negate-condition", Namespace, out value))
                IsNegate = value == "yes";

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    Text = reader.Value;
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            writer.WriteAttributeString("collation", Collation);
            writer.WriteAttributeString("negate-condition", IsNegate ? "yes" : "no");
            writer.WriteAttributeString("match-type", ConvertMatchTypeToString());
            writer.WriteString(Text);
            writer.WriteEndElement();
        }

        #endregion

        #region Helpers members

        string ConvertMatchTypeToString()
        {
            return ToKebabCase(MatchType.ToString());
        }

        readonly static Regex r = new("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])", RegexOptions.Compiled);
        static string ToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return r.Replace(value, "-$1")
                .Trim()
                .ToLower();
        }

        static string FromKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Trim().Replace("-", "");

        }

        #endregion
    }
}