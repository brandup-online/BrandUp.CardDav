namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface ICardDavRequest
    {
        IDictionary<string, string> Headers { get; }
        IRequestBody Body { get; }
    }

    public interface IHttpRequestConvertable
    {
        HttpRequestMessage ToHttpRequest();
    }

}
