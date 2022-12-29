using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(ElementName = "multistatus", Namespace = "DAV:")]
    public class ReportResponseBody : IResponseBody
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<AddressDataResource> Resources { get; private set; } = new List<AddressDataResource>();

        /// <summary>
        /// 
        /// </summary>
        public ReportResponseBody() { }

        #region IResponseBody members

        IList<IResponseResource> IResponseBody.Resources => (IList<IResponseResource>)Resources;

        #endregion

        #region IXmlSerializable members

        XmlSchema IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
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

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (IXmlSerializable resource in Resources)
            {
                resource.WriteXml(writer);
            }
        }

        #endregion
    }
}
