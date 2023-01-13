using BrandUp.CardDav.Transport.Abstract.Responces;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(Namespace = "DAV:", ElementName = "multistatus")]
    public class MultistatusResponseBody : IResponseBody
    {
        #region IResponseBody

        /// <summary>
        /// 
        /// </summary>
        public IList<IResponseResource> Resources { get; private set; } = new List<IResponseResource>();

        #endregion

        #region IDavProperty

        /// <summary>
        /// 
        /// </summary>
        public string Name => "multistatus";

        /// <summary>
        /// 
        /// </summary>
        public string Namespace => "DAV:";

        #endregion

        #region IXmlSerializable

        XmlSchema IXmlSerializable.GetSchema() => null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var resourseList = new List<ResponseResource>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var resourse = (IXmlSerializable)new ResponseResource();
                    resourse.ReadXml(reader);
                    resourseList.Add(resourse as ResponseResource);
                }
            }
            Resources = resourseList.Cast<IResponseResource>().ToArray();
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
