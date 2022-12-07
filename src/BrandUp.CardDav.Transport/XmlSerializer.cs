using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;

namespace BrandUp.CardDav.Transport
{
    internal class XmlSerializer
    {
        public XmlSerializer() { }

        public static string Serialize(string method, string @namespace, IRequestBody body)
        {
            var doc = new XmlDocument();
            var el = (XmlElement)doc.AppendChild(doc.CreateElement(method, @namespace));
            var inner = doc.CreateElement(body.Name, body.Namespace);

            foreach (var property in body.Properties)
            {
                inner.AppendChild(doc.CreateElement(property.Name, property.Namespace));
            }
            el.AppendChild(inner);

            return doc.OuterXml;
        }
    }
}
