using System.Xml.Serialization;

namespace BrandUp.CardDav.Attributes
{
    public class PropfindAttribute : XmlRootAttribute
    {
        public PropfindAttribute()
        {
            Namespace = "DAV:";
            ElementName = "propfind";
        }
    }
}
