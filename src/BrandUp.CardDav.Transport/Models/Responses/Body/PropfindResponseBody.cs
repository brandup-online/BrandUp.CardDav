using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    [XmlRoot(Namespace = "DAV:", ElementName = "multistatus")]
    public class PropfindResponseBody : IResponseBody
    {
        #region IResponseBody

        public IList<IResponseResource> Resources { get; private set; } = new List<IResponseResource>();

        #endregion

        #region IXmlSerializable

        XmlSchema IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var resourseList = new List<DefaultResponseResource>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var resourse = (IXmlSerializable)new DefaultResponseResource();
                    resourse.ReadXml(reader);
                    resourseList.Add(resourse as DefaultResponseResource);
                }
            }
            Resources = resourseList.ToArray();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var resource in Resources)
            {
                resource.WriteXml(writer);
            }
        }

        #endregion
    }



}
