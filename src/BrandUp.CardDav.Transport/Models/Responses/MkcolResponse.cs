using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class MkcolResponse : IResponse
    {
        public bool IsSuccess { get; init; }
        public int StatusCode { get; init; }

        public IResponseBody Content { get; init; }

        public static IResponse Create(HttpResponseMessage message)
        {
            return new MkcolResponse
            {
                IsSuccess = message.IsSuccessStatusCode,
                StatusCode = ((int)message.StatusCode),
            };
        }
    }
}
