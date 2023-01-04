using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class VCardResponse : IResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string ETag { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public VCardModel VCard { get; init; }

        #region IResponse members

        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int StatusCode { get; init; }

        IResponseBody IResponse.Content => null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResponse Create(HttpResponseMessage message)
        {
            var vCard = VCardParser.Parse(message.Content.ReadAsStream());

            return new VCardResponse
            {
                IsSuccess = message.IsSuccessStatusCode,
                StatusCode = ((int)message.StatusCode),
                ETag = message.Headers.ETag?.Tag,
                VCard = vCard
            };
        }

        #endregion
    }
}
