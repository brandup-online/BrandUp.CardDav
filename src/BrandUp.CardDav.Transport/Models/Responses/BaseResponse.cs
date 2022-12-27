using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class BaseResponse : IResponse
    {
        public bool IsSuccess { get; init; }
        public int StatusCode { get; init; }

        public IResponseBody Content => null;

        public static IResponse Create(HttpResponseMessage message)
        {
            return new BaseResponse { IsSuccess = message.IsSuccessStatusCode, StatusCode = ((int)message.StatusCode) };
        }
    }
}
