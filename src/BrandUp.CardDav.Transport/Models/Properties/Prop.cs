using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    public class PropList
    {
        public IEnumerable<IDavProperty> Properties { get; set; }
        public PropList()
        {
        }

        public static PropList Create(params IDavProperty[] props)
        {
            return new() { Properties = props };

        }
    }

    public static class Prop
    {
        #region Static members

        public static IDavProperty ETag => new DefaultProp("getetag");
        public static IDavProperty CTag => new DefaultProp("getctag", "http://calendarserver.org/ns/");
        public static IDavProperty CurrentUserPrincipal => new DefaultProp("current-user-principal");
        public static IDavProperty PrincipalUrl => new DefaultProp("principal-URL");
        public static IDavProperty CreationDate => new DefaultProp("creationdate");
        public static IDavProperty DisplayName => new DefaultProp("displayname");
        public static IDavProperty ContentLanguage => new DefaultProp("getcontentlanguage");
        public static IDavProperty ContentLength => new DefaultProp("getcontentlength");
        public static IDavProperty ContentType => new DefaultProp("getcontenttype");
        public static IDavProperty LastModified => new DefaultProp("getlastmodified");
        public static IDavProperty ResourceType => new DefaultProp("resourcetype");

        #endregion
    }
}

