using BrandUp.CardDav.Transport.Abstract.Properties;

namespace BrandUp.CardDav.Transport.Models.Properties
{
    /// <summary>
    /// 
    /// </summary>
    public class PropList
    {
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> Properties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PropList()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public static PropList Create(params IDavProperty[] props)
        {
            return new() { Properties = props };

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Prop
    {
        #region Static members

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty ETag => new DefaultProp("getetag");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty CTag => new DefaultProp("getctag", "http://calendarserver.org/ns/");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty CurrentUserPrincipal => new HrefProp("current-user-principal");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty PrincipalUrl => new DefaultProp("principal-URL");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty CreationDate => new DefaultProp("creationdate");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty DisplayName => new DefaultProp("displayname");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty ContentLanguage => new DefaultProp("getcontentlanguage");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty ContentLength => new DefaultProp("getcontentlength");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty ContentType => new DefaultProp("getcontenttype");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty LastModified => new DefaultProp("getlastmodified");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty ResourceType => new PropWithInnerProps("resourcetype");

        /// <summary>
        /// 
        /// </summary>
        public static IDavProperty AddressData => new AddressData();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="namespase"></param>
        /// <returns></returns>
        public static IDavProperty Create(string name, string namespase)
        {
            if (name == "resourcetype")
                return ResourceType;
            else if (name == "current-user-principal")
                return CurrentUserPrincipal;

            return new DefaultProp(name, namespase);
        }

        #endregion
    }
}

