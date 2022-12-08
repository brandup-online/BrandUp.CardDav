namespace BrandUp.CardDav.Transport
{
    public interface IXmlConvertMetadata
    {
        internal string Name { get; }
        internal string Namespace { get; }
        internal IEnumerable<IXmlConvertMetadata> Inner { get; }
    }
}