using BrandUp.CardDav.Client.Options;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Responses;
using BrandUp.CardDav.VCard;
using Microsoft.Extensions.Logging;

namespace BrandUp.CardDav.Client
{
    /// <summary>
    /// Client for cardDav requests 
    /// </summary>
    public class CardDavClient : ICardDavClient
    {
        readonly HttpClient httpClient;
        readonly ILogger<CardDavClient> logger;
        /// <summary>
        /// Constructor
        /// </summary>
        public CardDavClient(HttpClient httpClient, ILogger<CardDavClient> logger, CardDavOptions options)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            SetOptions(options);
        }

        #region ICardDavClient members

        /// <summary>
        /// Request a options from server. Executes to base URL. 
        /// </summary>
        /// <returns><see cref="OptionsResponse"/></returns>
        public async Task<OptionsResponse> OptionsAsync(CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Options;
            return await ProccesResponse<OptionsResponse>("", request, cancellationToken);
        }

        /// <summary>
        /// Gets a VCard from server
        /// </summary>
        /// <param name="endpoint">VCard resource endpoint</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="VCardResponse"/></returns>
        public async Task<VCardResponse> GetAsync(string endpoint, CancellationToken cancellationToken)
            => await ProccesResponse<VCardResponse>(endpoint, new HttpRequestMessage { Method = HttpMethod.Get }, cancellationToken);


        /// <summary>
        /// Executes a propfind request to server
        /// </summary>
        /// <param name="endpoint">Server endpoint</param>
        /// <param name="request">Propfind request <see cref="PropfindRequest"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="PropfindResponse"/></returns>
        public async Task<PropfindResponse> PropfindAsync(string endpoint, PropfindRequest request, CancellationToken cancellationToken = default)
            => await ProccesResponse<PropfindResponse>(endpoint, request, cancellationToken);

        /// <summary>
        /// Executes a report request to server
        /// </summary>
        /// <param name="endpoint">Server endpoint</param>
        /// <param name="request">Report request <see cref="PropfindRequest"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="ReportResponse"/></returns>
        public async Task<ReportResponse> ReportAsync(string endpoint, ReportRequest request, CancellationToken cancellationToken = default)
            => await ProccesResponse<ReportResponse>(endpoint, request, cancellationToken);

        /// <summary>
        /// Executes a mkcol request to server
        /// </summary>
        /// <param name="endpoint">Server endpoint</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MkcolResponse> MkcolAsync(string endpoint, CancellationToken cancellationToken = default)
            => await ProccesResponse<MkcolResponse>(endpoint, new MkcolRequest(), cancellationToken);

        /// <summary>
        /// Adds contact to a VCard collections
        /// </summary>
        /// <param name="endpoint">VCard resource endpoint</param>
        /// <param name="vCard">contact <see cref="VCardModel"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="BaseResponse"/></returns>
        public async Task<BaseResponse> AddContactAsync(string endpoint, VCardModel vCard, CancellationToken cancellationToken)
            => await ProccesResponse<BaseResponse>(endpoint, new HttpRequestMessage { Method = HttpMethod.Put, Content = new StringContent(vCard.ToString()) }, cancellationToken);

        /// <summary>
        /// Delete contact from a VCard collections
        /// </summary>
        /// <param name="endpoint">VCard resource endpoint</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="BaseResponse"/></returns>
        public async Task<BaseResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken)
             => await ProccesResponse<BaseResponse>(endpoint, new HttpRequestMessage { Method = HttpMethod.Delete }, cancellationToken);

        /// <summary>
        /// Updates contact to a VCard collections
        /// </summary>
        /// <param name="endpoint">VCard resource endpoint</param>
        /// <param name="vCard">contact to replace <see cref="VCardModel"/></param>
        /// <param name="ETag">Entity tag of contact</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="BaseResponse"/></returns>
        public async Task<BaseResponse> UpdateContactAsync(string endpoint, VCardModel vCard, string ETag, CancellationToken cancellationToken)
        {
            var message = new HttpRequestMessage { Method = HttpMethod.Put, Content = new StringContent(vCard.ToString()) };
            message.Headers.Add("If-Match", ETag);
            return await ProccesResponse<BaseResponse>(endpoint, message, cancellationToken);
        }

        #endregion

        #region Helpers

        private async Task<T> ProccesResponse<T>(string endpoint, IHttpRequestConvertable request, CancellationToken cancellationToken) where T : class, IResponse, new()
        {
            using var httpRequest = request.ToHttpRequest();
            if (httpRequest.Content != null)
                logger.LogDebug(await httpRequest.Content.ReadAsStringAsync());

            return await ProccesResponse<T>(endpoint, httpRequest, cancellationToken);
        }

        private async Task<T> ProccesResponse<T>(string endpoint, HttpRequestMessage request, CancellationToken cancellationToken) where T : class, IResponse, new()
        {
            using var response = await ExecuteAsync(endpoint, request, cancellationToken);
            logger.LogDebug(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                return (T)T.Create(response);
            }
            else
            {
                return new() { IsSuccess = false, StatusCode = (int)response.StatusCode };
            }
        }

        private async Task<HttpResponseMessage> ExecuteAsync(string endpoint, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            requestMessage.RequestUri = new Uri(endpoint, UriKind.Relative);

            return await httpClient.SendAsync(requestMessage, cancellationToken);
        }

        private void SetOptions(CardDavOptions options)
        {
            httpClient.BaseAddress = new Uri(options.BaseUrl);

            if (options is CardDavOAuthOptions)
            {
                var oauth = (CardDavOAuthOptions)options;
                httpClient.DefaultRequestHeaders.Authorization = new($"Bearer", oauth.AccessToken);
            }
            else if (options is CardDavCredentialsOptions)
            {
                var cred = (CardDavCredentialsOptions)options;
                httpClient.DefaultRequestHeaders.Authorization = new($"Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", cred.Login, cred.Password))));
            }
        }

        #endregion
    }
}
