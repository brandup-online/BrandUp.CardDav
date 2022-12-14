using BrandUp.CardDav.Transport.Models.Abstract;
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

        #endregion

        #region IXmlSerializable members

        XmlSchema IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var props = new List<CardProperty>();
            var depth = reader.Depth;
            while (reader.Read())
            {
                if (reader.Depth <= depth)
                {
                    vCardProperties = props;
                    return;
                }

                var value = reader.GetAttribute("name", Namespace);
                props.Add(Enum.Parse<CardProperty>(value, true));
            }
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
