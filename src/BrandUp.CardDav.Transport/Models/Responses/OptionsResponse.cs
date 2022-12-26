using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class OptionsResponse : IResponse
    {
        public bool IsSuccess { get; init; }
        public string StatusCode { get; init; }
        public string[] DavHeaderValue { get; init; }
        public string[] AllowHeaderValue { get; init; }

        public IResponseBody Content { get; init; } = null;

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
