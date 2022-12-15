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
        public bool IsNegate { get; init; } = false;

        public TextMatch() { }

        public static TextMatch Create(string matchSubstring, TextMatchType textMatchType, bool isNegate = false)
        {
            return new TextMatch() { Text = matchSubstring, IsNegate = isNegate, MatchType = textMatchType };
        }
        public static TextMatch Create(string matchSubstring, TextMatchType textMatchType, string collation, bool isNegate = false)
        {
            return new TextMatch() { Text = matchSubstring, IsNegate = isNegate, MatchType = textMatchType, Collation = collation };
        }

        #region IDavProperty

        public string Name => "text-match";

        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
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

        #region Helpers

        private string ConvertMatchTypeToString()
        {
            return ToKebabCase(MatchType.ToString());
        }

        readonly static Regex r = new("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])", RegexOptions.Compiled);
        public static string ToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return r.Replace(value, "-$1")
                .Trim()
                .ToLower();
        }

        #endregion
    }
}