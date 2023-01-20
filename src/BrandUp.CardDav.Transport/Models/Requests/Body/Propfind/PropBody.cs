using BrandUp.CardDav.Attributes;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Transport.Models.Properties;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Propfind
{
    /// <summary>
    /// Body of propfind request. 
    /// </summary>
    [Propfind]
    public class PropBody : IRequestBody
    {
        private string innerName;
        private string innerNamespace;

        #region IRequestBody members

        /// <summary>
        /// Requested properties.
        /// </summary>
        public IEnumerable<IDavProperty> Properties { get; set; }

        string IRequestBody.Name => "propfind";

        string IRequestBody.Namespace => "DAV:";

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public bool IsAllProp => innerName == "allprop";

        /// <summary>
        /// 
        /// </summary>
        public PropBody()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespace"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PropBody(string name, string @namespace = "DAV:")
        {
            this.innerName = name ?? throw new ArgumentNullException(nameof(name));
            this.innerNamespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        }

        #region IXmlSerializable region

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        async void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var baseDepth = reader.Depth;
            var props = new List<IDavProperty>();
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (Math.Abs(reader.Depth - baseDepth) > 1)
                    {
                        props.Add(Prop.Create(reader.LocalName, reader.NamespaceURI));
                    }
                    else if (Math.Abs(reader.Depth - baseDepth) == 1)
                    {
                        innerName = reader.LocalName;
                        innerNamespace = reader.NamespaceURI;
                    }
                }
            }
            Properties = props;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(innerName, innerNamespace);
            if (Properties != null)
                foreach (IDavProperty property in Properties)
                {
                    property.WriteXml(writer);
                }
            writer.WriteEndElement();
        }

        #endregion
    }
}
