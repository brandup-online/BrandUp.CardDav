using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class MkcolResponse : IResponse
    {
        public bool IsSuccess { get; init; }
        public string StatusCode { get; init; }

        public IResponseBody Body => null;

        public static IResponse Create(HttpResponseMessage message)
        {
            return new OptionsResponse
            {
                IsSuccess = message.IsSuccessStatusCode,
                StatusCode = message.StatusCode.ToString(),
                AllowHeaderValue = message.Content.Headers.Allow.ToArray(),
                DavHeaderValue = message.Headers.GetValues("DAV").SelectMany(s => s.Split(",")).ToArray()
            };
        }
    }
}
