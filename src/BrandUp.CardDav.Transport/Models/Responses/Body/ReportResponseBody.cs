using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    [XmlRoot(ElementName = "multistatus", Namespace = "DAV:")]
    public class ReportResponseBody : IResponseBody
    {
        public IList<AddressDataResource> Resources { get; private set; }
        public ReportResponseBody() { }

        #region IResponseBody

        IList<IResponseResource> IResponseBody.Resources => (IList<IResponseResource>)Resources;

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            var resourseList = new List<AddressDataResource>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var resourse = new AddressDataResource();
                    resourse.ReadXml(reader);
                    resourseList.Add(resourse);
                }
            }
            Resources = resourseList.ToArray();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var resource in Resources)
            {
                resource.WriteXml(writer);
            }
        }

        #endregion
    }
}
