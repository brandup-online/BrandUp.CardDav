namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IResponse
    {
        bool IsSuccess { get; init; }
        string StatusCode { get; init; }
        IResponseBody Body { get; }

        static abstract IResponse Create(HttpResponseMessage message);
    }
}
