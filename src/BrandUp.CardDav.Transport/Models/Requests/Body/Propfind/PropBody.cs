using BrandUp.CardDav.Attributes;
using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Helpers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests.Body.Propfind
{
    /// <summary>
    /// Body of propfind request. 
    /// </summary>
    [Propfind]
    public class PropBody : IRequestBody, IResponseCreator
    {
        private string name;
        private string @namespace;

        #region IRequestBody members

        /// <summary>
        /// Requested properties.
        /// </summary>
        public IEnumerable<IDavProperty> Properties { get; set; }

        #endregion

        #region IResponseCreator members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public IResponseBody CreateResponse(IDictionary<string, IDavDocument> collection)
        {
            var response = new MultistatusResponseBody();

            foreach (var pair in collection)
            {
                if (name == "allprop")
                {
                    (var found, var notFound) = ResponseResourseHelper.GeneratePropfindResource(pair.Value, Properties, true);

                    response.Resources.Add(new ResponseResource() { Endpoint = pair.Key, FoundProperties = found, NotFoundProperties = notFound });
                }
                else
                {
                    (var found, var notFound) = ResponseResourseHelper.GeneratePropfindResource(pair.Value, Properties);

                    response.Resources.Add(new ResponseResource() { Endpoint = pair.Key, FoundProperties = found, NotFoundProperties = notFound });
                }
            }

            return response;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Name => "propfind";

        /// <summary>
        /// 
        /// </summary>
        public string Namespace => "DAV:";

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
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
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
                        props.Add(new DefaultProp(reader.LocalName, reader.NamespaceURI));
                    }
                    else if (Math.Abs(reader.Depth - baseDepth) == 1)
                    {
                        name = reader.LocalName;
                        @namespace = reader.NamespaceURI;
                    }
                }
            }
            Properties = props;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(name, @namespace);
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
