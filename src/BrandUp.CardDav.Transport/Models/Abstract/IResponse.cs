namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IResponse
    {
        bool IsSuccess { get; init; }
        int StatusCode { get; init; }
        IResponseBody Content { get; }

        static abstract IResponse Create(HttpResponseMessage message);
    }
}
