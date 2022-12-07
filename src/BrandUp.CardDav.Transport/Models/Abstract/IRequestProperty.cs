namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IRequestProperty : IXmlConvertable
    {
        public IRequestProperty Inner { get; }
    }
}