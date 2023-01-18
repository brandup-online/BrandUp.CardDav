using BrandUp.CardDav.Transport.Abstract.Properties;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    internal class PropWithInnerProps : IDavProperty
    {
        public string Name { get; }

        public string Namespace { get; }

        public PropWithInnerProps(string name, string @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {

        }

        public void WriteXml(XmlWriter writer)
        {

        }

        public void WriteXmlWithValue(XmlWriter writer, string value)
        {

        }
    }
}
