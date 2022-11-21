using BrandUp.CardDav.Client.Options;
using Microsoft.Extensions.Logging;

namespace BrandUp.CardDav.Client
{
    public class CardDavClientFactory : ICardDavClientFactory
    {
        readonly IHttpClientFactory httpClientFactory;
        readonly ILogger<CardDavClient> logger;

        public CardDavClientFactory(IHttpClientFactory httpClientFactory, ILogger<CardDavClient> logger)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region ICardDavClientFactory

        public CardDavClient CreateClientWithAccessToken(CardDavOAuthOptions options)
        {
            throw new NotImplementedException();
        }

        public CardDavClient CreateClientWithCredentials(CardDavCredentialsOptions options)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public interface ICardDavClientFactory
    {
        CardDavClient CreateClientWithCredentials(CardDavCredentialsOptions options);
        CardDavClient CreateClientWithAccessToken(CardDavOAuthOptions options);
    }
}
