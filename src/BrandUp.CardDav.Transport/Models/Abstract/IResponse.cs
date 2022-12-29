namespace BrandUp.CardDav.Transport.Models.Abstract
{
    /// <summary>
    /// Response object
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Success flag
        /// </summary>
        bool IsSuccess { get; init; }

        /// <summary>
        /// Status code of response
        /// </summary>
        int StatusCode { get; init; }

        /// <summary>
        /// Response content
        /// </summary>
        IResponseBody Content { get; }

        /// <summary>
        /// Creating Response object from HttpResponseMessage
        /// </summary>
        /// <param name="message">Response message from server</param>
        /// <returns>IResponse object</returns>
        static abstract IResponse Create(HttpResponseMessage message);
    }
}
