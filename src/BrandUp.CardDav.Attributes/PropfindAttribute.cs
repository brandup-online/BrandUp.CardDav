using System.Xml.Serialization;

namespace BrandUp.CardDav.Attributes
{
    /// <summary>
    /// Defines a propfind xml root
    /// </summary>
    public class PropfindAttribute : XmlRootAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public PropfindAttribute()
        {
            Namespace = "DAV:";
            ElementName = "propfind";
        }
    }
}
