using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    internal class PropFilter : IDavProperty
    {
        #region IDavProperty members

        public string Name => throw new NotImplementedException();

        public string Namespace => throw new NotImplementedException();

        #endregion

        #region IXmlSerializable members

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}