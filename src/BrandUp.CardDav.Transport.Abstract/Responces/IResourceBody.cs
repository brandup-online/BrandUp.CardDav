using BrandUp.CardDav.Transport.Abstract.Properties;

namespace BrandUp.CardDav.Transport.Abstract.Responces
{
    public interface IResourceBody
    {
        public IDavProperty DavProperty { get; }

        public string Value { get; }

        public bool IsFound { get; }
    }
}