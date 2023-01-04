using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseResponse : IResponse
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
        public IResponseBody Content => null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResponse Create(HttpResponseMessage message)
        {
            return new BaseResponse { IsSuccess = message.IsSuccessStatusCode, StatusCode = ((int)message.StatusCode) };
        }
    }
}
