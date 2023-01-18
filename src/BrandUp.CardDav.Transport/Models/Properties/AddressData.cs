using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    /// <summary>
    /// 
    /// </summary>
    public class AddressData : IDavProperty
    {
        private IEnumerable<CardProperty> vCardProperties;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<CardProperty> VCardProperies => vCardProperties;

        /// <summary>
        /// 
        /// </summary>
        public AddressData() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        public AddressData(params CardProperty[] props)
        {
            vCardProperties = props?.Distinct().ToList() ?? new List<CardProperty>();
        }

        #region IDavProperty members

        /// <summary>
        /// 
        /// </summary>
        public string Name => "address-data";

        /// <summary>
        /// 
        /// </summary>
        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public void WriteXmlWithValue(XmlWriter writer, string value)
        {
            var vcard = new VCardModel(value);

            string result;
            if (vCardProperties != null && !vCardProperties.Any())
                result = vcard.ToString();
            else result = vcard.ToStringProps(vCardProperties);

            writer.WriteElementString(Name, Namespace, result);
        }

        #endregion

        #region IXmlSerializable members

        XmlSchema IXmlSerializable.GetSchema() => null;

        async void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var props = new List<CardProperty>();

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Whitespace)
                    continue;

                if (reader.LocalName == Name && reader.NamespaceURI == Namespace)
                {
                    if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    else if (reader.NodeType == XmlNodeType.Element)
                        continue;
                }
                else if (reader.LocalName.Equals("prop", StringComparison.CurrentCultureIgnoreCase) && reader.NamespaceURI == Namespace)
                {
                    if (reader.TryGetAttribute("name", Namespace, out var value))
                        if (Enum.TryParse<CardProperty>(value, true, out var enumVal))
                            props.Add(enumVal);
                }
                else
                {
                    break;
                }
            }
            vCardProperties = props;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            if (vCardProperties != null)
                foreach (CardProperty property in vCardProperties)
                {
                    writer.WriteStartElement("C", "prop", Namespace);

                    writer.WriteAttributeString("C", "name", Namespace, property.ToString());

                    writer.WriteEndElement();
                }
            writer.WriteEndElement();
        }

        #endregion
    }
}
