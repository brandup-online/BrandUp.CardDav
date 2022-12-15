using BrandUp.CardDav.Client.Options;
using BrandUp.CardDav.Transport;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Responses;
using BrandUp.CardDav.VCard;
using Microsoft.Extensions.Logging;

namespace BrandUp.CardDav.Client
{
    public class CardDavClient : ICardDavClient
    {
        readonly HttpClient httpClient;
        readonly ILogger<CardDavClient> logger;

        public CardDavClient(HttpClient httpClient, ILogger<CardDavClient> logger, CardDavOptions options)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            SetOptions(options);
        }

        #region ICardDavClient members

        public async Task<OptionsResponse> OptionsAsync(CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Options;
            return await ProccesResponse<OptionsResponse>("", request, cancellationToken);
        }

        // Must be vcard adress
        public async Task<VCardModel> GetAsync(string endpoint, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync(endpoint, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                if (VCardParser.TryParse(await response.Content.ReadAsStringAsync(cancellationToken), out var vCard))
                {
                    return vCard;
                }
                else
                {
                    throw new ArgumentException("Endpoint response is not a vCard");
                }
            }
            else
            {
                throw new HttpRequestException(response.StatusCode.ToString());
            }

        }

        public async Task<PropfindResponse> PropfindAsync(string endpoint, PropfindRequest request, CancellationToken cancellationToken = default)
             => await ProccesResponse<PropfindResponse>(endpoint, request, cancellationToken);

        public async Task<ReportResponse> ReportAsync(string endpoint, ReportRequest request, CancellationToken cancellationToken = default)
             => await ProccesResponse<ReportResponse>(endpoint, request, cancellationToken);

        public async Task<MkcolResponse> MkcolAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage();
            request.Method = new("MKCOL");
            return await ProccesResponse<MkcolResponse>(endpoint, request, cancellationToken);
        }

        public async Task<CarddavResponse> AddContactAsync(string endpoint, VCardModel vCard, CancellationToken cancellationToken)
            => ProccesCardDavResponse(await ExecuteAsync(endpoint, HttpMethod.Put, await VCardSerializer.SerializeAsync(vCard, cancellationToken), null, cancellationToken));

        public async Task<CarddavResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken)
            => ProccesCardDavResponse(await ExecuteAsync(endpoint, HttpMethod.Delete, null, null, cancellationToken));

        public async Task<CarddavResponse> UpdateContactAsync(string endpoint, VCardModel vCard, string ETag, CancellationToken cancellationToken)
            => ProccesCardDavResponse(await ExecuteAsync(endpoint, HttpMethod.Put, await VCardSerializer.SerializeAsync(vCard, cancellationToken), new() { { "If-Match", ETag } }, cancellationToken));

        #endregion

        #region Helpers

        private async Task<HttpResponseMessage> ExecuteAsync(string endpoint, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            requestMessage.RequestUri = new Uri(endpoint, UriKind.Relative);

            return await httpClient.SendAsync(requestMessage, cancellationToken);
        }

        private async Task<T> ProccesResponse<T>(string endpoint, ICardDavRequest request, CancellationToken cancellationToken) where T : class, IResponse, new()
        {
            using var httpRequest = request.ToHttpRequest();
            return await ProccesResponse<T>(endpoint, httpRequest, cancellationToken);
        }

        private async Task<T> ProccesResponse<T>(string endpoint, HttpRequestMessage request, CancellationToken cancellationToken) where T : class, IResponse, new()
        {
            using var response = await ExecuteAsync(endpoint, request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return (T)T.Create(response);
            }
            else
            {
                return new() { IsSuccess = false, StatusCode = response.StatusCode.ToString() };
            }
        }

        private bool IsSuccessXmlResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return false;

            if (response.Content.Headers.ContentType == null)
                return false;

            if (!response.Content.Headers.ContentType.MediaType.Contains("xml"))
                return false;

            if (response.Content.Headers.ContentLength == 0)
                return false;

            return true;
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

        #region Obsolete

        private async Task<HttpResponseMessage> ExecuteAsync(string endpoint, HttpMethod method, string request = null, Dictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            using var requestMessage = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                requestMessage.Content = new StringContent(request);
                if (headers != null)
                {
                    if (headers.TryGetValue("Content-Type", out var contentType))
                    {

                        requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                        headers.Remove("Content-Type");
                    }

                    if (headers.TryGetValue("If-Match", out var match))
                    {
                        requestMessage.Headers.Add("If-Match", match);
                        headers.Remove("If-Match");
                    }

                    foreach (var header in headers)
                        requestMessage.Content.Headers.Add(header.Key, header.Value);
                }
            }

            return await ExecuteAsync(requestMessage, cancellationToken);
        }

        private async Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            requestMessage.Headers.Authorization = httpClient.DefaultRequestHeaders.Authorization;
            return await httpClient.SendAsync(requestMessage, cancellationToken);
        }

        private CarddavResponse ProccesCardDavResponse(HttpResponseMessage response)
        {
            var carddavResponse = new CarddavResponse
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode.ToString()
            };

            if (response.IsSuccessStatusCode)
            {
                carddavResponse.ETag = response.Headers.ETag?.Tag;

                if (response.StatusCode == System.Net.HttpStatusCode.MultiStatus && response.Content.Headers.ContentType.MediaType.Contains("xml"))
                {
                    carddavResponse.Content = XmlParser.GenerateCarddavContent(response.Content.ReadAsStream());
                }

            }

            response.Dispose();
            return carddavResponse;
        }

        #endregion

        #endregion
    }
}
