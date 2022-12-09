using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Body
{
    internal class AllProp : IRequestBody
    {
        IEnumerable<IDavProperty> IRequestBody.Properties => null;

        string IXmlConvertMetadata.Name => "allprop";

        string IXmlConvertMetadata.Namespace => "DAV:";

        IEnumerable<IXmlConvertMetadata> IXmlConvertMetadata.Inner => null;
    }
}
