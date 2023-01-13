namespace BrandUp.CardDav.Transport.Abstract.Requests
{
    /// <summary>
    /// Request object.
    /// </summary>
    public interface ICardDavRequest
    {
        /// <summary>
        /// Request headers.
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Request body
        /// </summary>
        IRequestBody Body { get; }
    }

    /// <summary>
    /// Object than implement this inteface can convert self to HttpRequestMessage
    /// </summary>
    public interface IHttpRequestConvertable
    {
        /// <summary>
        /// Convert this object to HttpRequestMessage
        /// </summary>
        /// <returns>Request</returns>
        HttpRequestMessage ToHttpRequest();
    }

}
