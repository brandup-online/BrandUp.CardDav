namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IDavProperty : IXmlConvertMetadata, IPropertyKey, IEquatable<IDavProperty>
    {
    }

    public interface IPropertyKey
    {
        internal string Key { get; }
    }
}