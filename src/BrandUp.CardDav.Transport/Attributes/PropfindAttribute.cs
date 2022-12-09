using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Attributes
{
    internal class PropfindAttribute : XmlRootAttribute
    {
        public PropfindAttribute()
        {
            Namespace = "DAV:";
            ElementName = "propfind";
        }
    }
}
