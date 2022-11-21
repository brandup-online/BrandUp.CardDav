using BrandUp.CardDav.Client.Options;
using Microsoft.Extensions.Logging;

namespace BrandUp.CardDav.Client
{
    public class CardDavClient
    {
        readonly HttpClient httpClient;
        readonly ILogger<CardDavClient> logger;
        readonly string baseUrl;

        public CardDavClient(HttpClient httpClient, ILogger<CardDavClient> logger, CardDavOptions options)
        {
            if (options == null)
                throw new ArgumentNullException();

            baseUrl = options.BaseUrl ?? throw new ArgumentNullException(nameof(baseUrl));

            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
