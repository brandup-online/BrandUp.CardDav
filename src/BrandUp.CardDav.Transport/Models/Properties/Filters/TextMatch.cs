using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    public class TextMatch : ICondition
    {
        public string Text { get; set; }
        public string Collation { get; set; } = "i;unicode-casemap";
        public TextMatchType MatchType { get; set; }
        public bool IsNegate { get; set; } = false;

        public TextMatch() { }

        public static TextMatch Create(string matchSubstring, TextMatchType textMatchType, bool isNegate = false)
        {
            return new TextMatch() { Text = matchSubstring, IsNegate = isNegate, MatchType = textMatchType };
        }
        public static TextMatch Create(string matchSubstring, TextMatchType textMatchType, string collation, bool isNegate = false)
        {
            return new TextMatch() { Text = matchSubstring, IsNegate = isNegate, MatchType = textMatchType, Collation = collation };
        }

        #region ICondition members

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

        public string Name => "text-match";

        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Attribute)
                {
                    if (reader.LocalName == "collation")
                        Collation = reader.Value;
                    if (reader.LocalName == "match-type")
                    {
                        MatchType = Enum.Parse<TextMatchType>(reader.Value, true);
                    }
                    if (reader.LocalName == "negate-condition")
                    {
                        IsNegate = reader.Value == "yes";
                    }
                }

                if (reader.NodeType == XmlNodeType.Text)
                {
                    Text = reader.Value;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
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