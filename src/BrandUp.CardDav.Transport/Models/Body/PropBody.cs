using BrandUp.CardDav.Transport.Attributes;
using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml;
using System.Xml.Schema;

namespace BrandUp.CardDav.Transport.Models.Body
{
    [Propfind]
    public class PropBody : IRequestBody
    {
        #region IRequestBody members

        public IEnumerable<IDavProperty> Properties { get; set; }

        #endregion

        public string Name => "prop";

        public string Namespace => "DAV:";

        #region IXmlSerializable region

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadToFollowing("prop", "DAV:");
            var props = new List<IDavProperty>();
            while (reader.Read())
            {
                props.Add(new Prop(reader.LocalName, reader.NamespaceURI));
            }
            Properties = props;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name, Namespace);
            foreach (IDavProperty property in Properties)
            {
                property.WriteXml(writer);
            }
            writer.WriteEndElement();
        }

        #endregion
    }

    public class Prop : IDavProperty
    {
        private string name;
        private string @namespace;

        internal Prop(string name, string @namespace = "DAV:")
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        }

        #region IDavProperty members
        public string Name => name;

        public string Namespace => @namespace;

        #endregion

        #region IXmlSerializable

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(Name, Namespace, "");
        }

        #endregion

        #region Static members

        public static Prop ETag => new("getetag");
        public static Prop CTag => new("getctag", "http://calendarserver.org/ns/");
        public static Prop CurrentUserPrincipal => new("current-user-principal");
        public static Prop PrincipalUrl => new("principal-URL");
        public static Prop CreationDate => new("creationdate");
        public static Prop DisplayName => new("displayname");
        public static Prop ContentLanguage => new("getcontentlanguage");
        public static Prop ContentLength => new("getcontentlength");
        public static Prop ContentType => new("getcontenttype");
        public static Prop LastModified => new("getlastmodified");
        public static Prop ResourceType => new("resourcetype");

        #endregion
    }
}
