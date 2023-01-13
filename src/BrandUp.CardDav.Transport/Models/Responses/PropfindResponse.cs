using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using BrandUp.CardDav.Xml;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class PropfindResponse : IResponse
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
        public MultistatusResponseBody Body { get; init; }

        IResponseBody IResponse.Content => Body;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResponse Create(HttpResponseMessage message)
        {
            var body = CustomSerializer.DeserializeResponse<MultistatusResponseBody>(message.Content.ReadAsStream());
            var response = new PropfindResponse { IsSuccess = message.IsSuccessStatusCode, StatusCode = ((int)message.StatusCode), Body = body as MultistatusResponseBody };

            return response;
        }
    }
}

