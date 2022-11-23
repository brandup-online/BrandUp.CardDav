using BrandUp.CardDav.Client.Models;
using BrandUp.CardDav.Client.Options;
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

        public Task<string> OptionsAsync(CancellationToken cancellationToken)
            => ExecuteAsync("", HttpMethod.Options, cancellationToken);

        // Must be vcard adress
        public Task<string> GetAsync(string endpoint, CancellationToken cancellationToken)
            => ExecuteAsync(endpoint, HttpMethod.Get, cancellationToken);

        public Task<string> PropfindAsync(string endpoint, CarddavRequest request, CancellationToken cancellationToken)
             => ExecuteAsync(endpoint, new HttpMethod("PROPFIND"), request, cancellationToken);

        public Task<string> ReportAsync(string endpoint, CarddavRequest request, CancellationToken cancellationToken)
             => ExecuteAsync(endpoint, new HttpMethod("REPORT"), request, cancellationToken);

        public Task<string> AddContactAsync(string endpoint, string vCard, CancellationToken cancellationToken)
            => ExecuteAsync(endpoint, HttpMethod.Put, vCard, cancellationToken);

        public Task<string> DeleteContactAsync(string endpoint, string vCard, CancellationToken cancellationToken)
            => ExecuteAsync(endpoint, HttpMethod.Put, vCard, cancellationToken);

        public Task<string> UpdateContactAsync(string endpoint, string vCard, CancellationToken cancellationToken)
            => ExecuteAsync(endpoint, HttpMethod.Put, vCard, cancellationToken);

        #endregion

        #region Helpers

        private async Task<string> ExecuteAsync(string endpoint, HttpMethod method, CarddavRequest request, CancellationToken cancellationToken)
        {
            using var requestMessage = new HttpRequestMessage(method, endpoint);
            if (request != null)
            {
                requestMessage.Content = new StringContent(request.XmlContent ?? "");
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
                requestMessage.Content.Headers.Add("Depth", request.Depth);
            }

            using var response = await httpClient.SendAsync(requestMessage, cancellationToken);

            if (response.Content.Headers.ContentLength != 0)
                return await response.Content.ReadAsStringAsync(cancellationToken);
            else return response.Headers.ToString();
        }

        private async Task<string> ExecuteAsync(string endpoint, HttpMethod method, string vCard, CancellationToken cancellationToken)
        {
            using var requestMessage = new HttpRequestMessage(method, endpoint);
            if (vCard != null)
            {
                requestMessage.Content = new StringContent(vCard);
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/vcard");
            }

            using var response = await httpClient.SendAsync(requestMessage, cancellationToken);

            if (response.Content.Headers.ContentLength != 0)
                return await response.Content.ReadAsStringAsync(cancellationToken);
            else return response.Headers.ToString();
        }

        private async Task<string> ExecuteAsync(string endpoint, HttpMethod method, CancellationToken cancellationToken)
        {
            using var requestMessage = new HttpRequestMessage(method, endpoint);

            using var response = await httpClient.SendAsync(requestMessage, cancellationToken);

            if (response.Content.Headers.ContentLength != 0)
                return await response.Content.ReadAsStringAsync(cancellationToken);
            else return response.Headers.ToString();
        }

        private void SetOptions(CardDavOptions options)
        {
            httpClient.BaseAddress = new Uri(options.BaseUrl);

            if (options is CardDavOAuthOptions)
            {
                var oauth = (CardDavOAuthOptions)options;
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {oauth.AccessToken}");
            }
            else if (options is CardDavCredentialsOptions)
            {
                var cred = (CardDavCredentialsOptions)options;
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", cred.Login, cred.Password))));
            }
        }

        #endregion
    }
}
