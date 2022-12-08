namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IResponse
    {
        bool IsSuccess { get; }
        string StatusCode { get; }
        IEnumerable<IResponseResource> Resources { get; }
    }


}
