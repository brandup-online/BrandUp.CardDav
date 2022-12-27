using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    [XmlRoot(ElementName = "multistatus", Namespace = "DAV:")]
    public class ReportResponseBody : IResponseBody
    {
        public IList<AddressDataResource> Resources { get; private set; } = new List<AddressDataResource>();
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
                    var resourse = (IXmlSerializable)new AddressDataResource();
                    resourse.ReadXml(reader);
                    resourseList.Add(resourse as AddressDataResource);
                }
            }
            Resources = resourseList.ToArray();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (IXmlSerializable resource in Resources)
            {
                resource.WriteXml(writer);
            }
        }

        #endregion
    }
}
