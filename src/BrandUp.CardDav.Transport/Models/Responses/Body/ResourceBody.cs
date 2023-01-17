using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Responces;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    internal class ResourceBody : IResourceBody
    {
        public IDavProperty DavProperty { get; init; }

        public string Value { get; init; }

        public bool IsFound { get; init; }
    }
}
