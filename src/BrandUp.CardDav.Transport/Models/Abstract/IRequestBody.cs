namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IRequestBody : IXmlConvertMetadata
    {
        IEnumerable<IDavProperty> Properties { get; }
    }

    public interface IRequestFilter
    {

    }
}
