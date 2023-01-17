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
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public void WriteXmlWithValue(XmlWriter writer, string value)
        {
            throw new NotImplementedException();
        }
    }
}
