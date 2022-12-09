using BrandUp.CardDav.Transport.Attributes;
using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Body
{
    [Propfind]
    public class AllProp : IRequestBody
    {
        IEnumerable<IDavProperty> IRequestBody.Properties => null;

        string Name => "allprop";

        string Namespace => "DAV:";

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {

        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            writer.WriteEndElement();
        }
    }
}
