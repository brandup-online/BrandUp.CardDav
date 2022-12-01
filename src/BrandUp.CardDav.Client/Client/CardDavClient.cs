using BrandUp.CardDav.Client.Models.Responses;
using BrandUp.CardDav.Client.Options;
using BrandUp.CardDav.Client.Xml;
using BrandUp.VCard;
using Microsoft.Extensions.Logging;

namespace BrandUp.CardDav.Client.Client
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

        public Task<CarddavResponse> OptionsAsync(CancellationToken cancellationToken)
            => ExecuteAsync("", HttpMethod.Options, cancellationToken);

        // Must be vcard adress
        public Task<CarddavResponse> GetAsync(string endpoint, CancellationToken cancellationToken)
            => ExecuteAsync(endpoint, HttpMethod.Get, cancellationToken);

        public Task<CarddavResponse> PropfindAsync(string endpoint, string xmlRequest, string depth = "0", CancellationToken cancellationToken = default)
             => ExecuteAsync(endpoint, new HttpMethod("PROPFIND"), xmlRequest, new() { { "Depth", depth } }, cancellationToken);

        public Task<CarddavResponse> ReportAsync(string endpoint, string xmlRequest, string depth = "0", CancellationToken cancellationToken = default)
             => ExecuteAsync(endpoint, new HttpMethod("REPORT"), xmlRequest, new() { { "Depth", depth } }, cancellationToken);

        public Task<CarddavResponse> AddContactAsync(string endpoint, VCard.VCard vCard, CancellationToken cancellationToken)
            => ExecuteAsync(endpoint, HttpMethod.Put, vCard, null, cancellationToken);

        public Task<CarddavResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken)
            => ExecuteAsync(endpoint, HttpMethod.Delete, cancellationToken);

        public Task<CarddavResponse> UpdateContactAsync(string endpoint, VCard.VCard vCard, string ETag, CancellationToken cancellationToken)
            => ExecuteAsync(endpoint, HttpMethod.Put, vCard, ETag, cancellationToken);

        #endregion

        #region Helpers

        private async Task<CarddavResponse> ExecuteAsync(string endpoint, HttpMethod method, string request, Dictionary<string, string> headers, CancellationToken cancellationToken)
        {
            using var requestMessage = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                requestMessage.Content = new StringContent(request);
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");

                foreach (var header in headers)
                    requestMessage.Content.Headers.Add(header.Key, header.Value);
            }

            return await ExecuteAsync(requestMessage, cancellationToken);
        }

        private async Task<CarddavResponse> ExecuteAsync(string endpoint, HttpMethod method, VCard.VCard vCard, string eTag = null, CancellationToken cancellationToken = default)
        {
            using var requestMessage = new HttpRequestMessage(method, endpoint);
            if (vCard != null)
            {
                requestMessage.Content = new StringContent(await VCardSerializer.SerializeAsync(vCard, cancellationToken));
                if (eTag != null)
                    requestMessage.Headers.Add("If-Match", eTag);
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/vcard");
            }

            return await ExecuteAsync(requestMessage, cancellationToken);
        }

        private async Task<CarddavResponse> ExecuteAsync(string endpoint, HttpMethod method, CancellationToken cancellationToken)
        {
            using var requestMessage = new HttpRequestMessage(method, endpoint);

            return await ExecuteAsync(requestMessage, cancellationToken);
        }

        private async Task<CarddavResponse> ExecuteAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            requestMessage.Headers.Authorization = httpClient.DefaultRequestHeaders.Authorization;
            using var response = await httpClient.SendAsync(requestMessage, cancellationToken);

            if (response.Content.Headers.ContentLength != 0)
            {
                if (response.Content.Headers.ContentType.MediaType.Contains("xml"))
                {
                    var parser = new XmlParser(await response.Content.ReadAsStreamAsync(cancellationToken));
                    return parser.GenerateCarddavResponse();
                }
                else if (response.Content.Headers.ContentType.MediaType.Contains("card") || response.Content.Headers.ContentType.MediaType.Contains("text"))
                {
                    var cardavResponse = new CarddavResponse
                    {
                        IsSuccess = response.IsSuccessStatusCode,
                        StatusCode = response.StatusCode.ToString(),

                    };
                    cardavResponse.VCardResponse.Add(new()
                    {
                        Etag = response.Headers.ETag.Tag,
                        Endpoint = requestMessage.RequestUri.ToString(),
                        VCard = await VCardSerializer.DeserializeAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken)
                    });
                    return cardavResponse;
                }
                else
                {
                    return new CarddavResponse
                    {
                        IsSuccess = response.IsSuccessStatusCode,
                        StatusCode = response.StatusCode.ToString(),
                    };
                }
            }
            else
            {
                return new CarddavResponse
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode.ToString(),
                    ETag = response.Headers.ETag?.Tag
                };
            }
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
