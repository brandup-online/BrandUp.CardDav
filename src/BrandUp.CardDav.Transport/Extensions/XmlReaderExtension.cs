using System.Xml;

namespace BrandUp.CardDav.Transport
{
    internal static class XmlReaderExtension
    {
        public static bool TryGetAttribute(this XmlReader reader, string name, string @namespase, out string value)
        {
            value = reader.GetAttribute(name);
            if (value != null)
                return true;

            value = reader.GetAttribute(name, namespase);
            if (value != null)
                return true;

            value = reader.GetAttribute($"{reader.Prefix}:{name}", namespase);
            if (value != null)
                return true;

            return false;
        }
    }
}