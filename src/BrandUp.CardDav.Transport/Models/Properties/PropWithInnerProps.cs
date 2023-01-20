using BrandUp.CardDav.Transport.Abstract.Properties;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    internal class PropWithInnerProps : IDavProperty
    {

        public PropWithInnerProps(string name, string @namespace = "DAV:")
        {
            Name = name;
            Namespace = @namespace;
        }

        #region IDavProperty members

        public string Name { get; }

        public string Namespace { get; }

        public void WriteXmlWithValue(XmlWriter writer, string value)
        {
            writer.WriteStartElement(Name, Namespace);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(value);
            xmlDoc.WriteTo(writer);

            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable members

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {

        }

        public void WriteXml(XmlWriter writer)
        {

        }

        #endregion
    }
}
