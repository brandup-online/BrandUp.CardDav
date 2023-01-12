using BrandUp.CardDav.Attributes;
using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Helpers;
using BrandUp.CardDav.Transport.Models.Abstract;
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
            var response = new PropfindResponseBody();

            foreach (var pair in collection)
            {
                (var found, var notFound) = ResponseResourseHelper.GeneratePropfindResource(pair.Value, Properties);

                response.Resources.Add(new DefaultResponseResource() { Endpoint = pair.Key, FoundProperties = found, NotFoundProperties = notFound });
            }

            return response;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string Name => name;

        /// <summary>
        /// 
        /// </summary>
        public string Namespace => @namespace;

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

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            name = reader.LocalName;
            @namespace = reader.NamespaceURI;
            var props = new List<IDavProperty>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.LocalName != "prop")
                    props.Add(new DefaultProp(reader.LocalName, reader.NamespaceURI));
            }
            Properties = props;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
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
