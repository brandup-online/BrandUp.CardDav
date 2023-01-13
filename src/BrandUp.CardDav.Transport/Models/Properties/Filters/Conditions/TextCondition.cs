using BrandUp.CardDav.Transport.Abstract.Enum;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters.Conditions
{
    internal class TextCondition : IXmlSerializable
    {
        private string text;
        private string collation = "i;unicode-casemap";
        private TextMatchType matchType;
        private bool isNegate = false;

        internal TextCondition()
        { }

        public TextCondition(string text, TextMatchType matchType, bool isNegate = false, string collation = "i;unicode-casemap")
        {
            this.text = text ?? throw new ArgumentNullException(nameof(text));
            this.matchType = matchType;
            this.isNegate = isNegate;
            this.collation = collation;
        }

        public bool Check(string value)
        {
            switch (matchType)
            {
                case TextMatchType.Equals: return isNegate ^ value.Equals(text, StringComparison.OrdinalIgnoreCase);
                case TextMatchType.Contains: return isNegate ^ value.Contains(text, StringComparison.OrdinalIgnoreCase);
                case TextMatchType.StartsWith: return isNegate ^ value.StartsWith(text, StringComparison.OrdinalIgnoreCase);
                case TextMatchType.EndsWith: return isNegate ^ value.EndsWith(text, StringComparison.OrdinalIgnoreCase);
                default: throw new ArgumentException("Unexpected value of enum");
            }
        }

        #region IXmlSerializable members

        private const string name = "text-match";
        private const string @namespace = "urn:ietf:params:xml:ns:carddav";

        public XmlSchema GetSchema() => null;

        async public void ReadXml(XmlReader reader)
        {
            if (reader.TryGetAttribute("collation", @namespace, out var value))
                collation = value;

            if (reader.TryGetAttribute("match-type", @namespace, out value))
                matchType = Enum.Parse<TextMatchType>(FromKebabCase(value), true);

            if (reader.TryGetAttribute("negate-condition", @namespace, out value))
                isNegate = value == "yes";

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    text = reader.Value;
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(name, @namespace);
            writer.WriteAttributeString("collation", collation);
            writer.WriteAttributeString("negate-condition", isNegate ? "yes" : "no");
            writer.WriteAttributeString("match-type", ConvertMatchTypeToString());
            writer.WriteString(text);
            writer.WriteEndElement();
        }

        #endregion

        #region Helpers members

        string ConvertMatchTypeToString()
        {
            return ToKebabCase(matchType.ToString());
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
