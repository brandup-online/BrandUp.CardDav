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

        public IList<IResponseResource> Resources { get; private set; }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            var resourseList = new List<DefaultResponseResource>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var resourse = new DefaultResponseResource();
                    resourse.ReadXml(reader);
                    resourseList.Add(resourse);
                }
            }
            Resources = resourseList.ToArray();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }



}
