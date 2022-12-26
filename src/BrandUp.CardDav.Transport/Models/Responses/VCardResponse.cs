using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class VCardResponse : IResponse
    {
        public string ETag { get; init; }
        public VCardModel VCard { get; init; }

        #region IResponse members

        public bool IsSuccess { get; init; }
        public string StatusCode { get; init; }

        IResponseBody IResponse.Content => null;

        public static IResponse Create(HttpResponseMessage message)
        {
            var vCard = VCardParser.Parse(message.Content.ReadAsStream());

            return new VCardResponse
            {
                IsSuccess = message.IsSuccessStatusCode,
                StatusCode = message.StatusCode.ToString(),
                ETag = message.Headers.ETag?.Tag,
                VCard = vCard
            };
        }

        #endregion
    }
}
