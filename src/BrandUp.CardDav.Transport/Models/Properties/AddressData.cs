using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    public class AddressData : IDavProperty
    {
        private IEnumerable<VCardProperty> vCardProperties;
        public IEnumerable<VCardProperty> VCardProperies => vCardProperties;
        public AddressData() { }
        public AddressData(params VCardProperty[] props)
        {
            vCardProperties = props?.Distinct().ToList() ?? new List<VCardProperty>();
        }

        #region IDavProperty members

        public string Name => "address-data";

        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        #endregion

        #region IXmlSerializable members

        XmlSchema IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var props = new List<VCardProperty>();
            var depth = reader.Depth;
            while (reader.Read())
            {
                if (reader.Depth <= depth)
                {
                    vCardProperties = props;
                    return;
                }

                var value = reader.GetAttribute("name", Namespace);
                props.Add(Enum.Parse<VCardProperty>(value, true));
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            if (vCardProperties != null)
                foreach (VCardProperty property in vCardProperties)
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
