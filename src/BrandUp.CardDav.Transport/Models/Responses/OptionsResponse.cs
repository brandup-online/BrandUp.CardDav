using BrandUp.CardDav.Transport.Abstract.Responces;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class OptionsResponse : IResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int StatusCode { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string[] DavHeaderValue { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public string[] AllowHeaderValue { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public IResponseBody Content { get; init; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResponse Create(HttpResponseMessage message)
        {
            return new OptionsResponse
            {
                IsSuccess = message.IsSuccessStatusCode,
                StatusCode = ((int)message.StatusCode),
                AllowHeaderValue = message.Content.Headers.Allow.ToArray(),
                DavHeaderValue = message.Headers.GetValues("DAV").SelectMany(s => s.Split(",")).ToArray()
            };
        }
    }
}
