﻿using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.VCard;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    internal class AddressData : IDavProperty
    {
        private IEnumerable<VCardProperty> vCardProperties;
        public AddressData() { }
        public AddressData(params VCardProperty[] props)
        {
            vCardProperties = props?.Distinct() ?? throw new ArgumentNullException(nameof(props));
        }

        #region IDavProperty members

        public string Name => "address-data";

        public string Namespace => "urn:ietf:params:xml:ns:carddav";

        #endregion

        #region IXmlSerializable members

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            var props = new List<VCardProperty>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "address-data")
                    return;

                if (reader.NodeType == XmlNodeType.Attribute && reader.LocalName == "name")
                {
                    props.Add(Enum.Parse<VCardProperty>(reader.Value, true));
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            foreach (VCardProperty property in vCardProperties)
            {
                writer.WriteStartElement("prop", Namespace);

                writer.WriteAttributeString("name", Namespace, property.ToString());

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}