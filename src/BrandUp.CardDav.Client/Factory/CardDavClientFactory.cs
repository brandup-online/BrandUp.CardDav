using BrandUp.CardDav.Client.Options;
using Microsoft.Extensions.Logging;

namespace BrandUp.CardDav.Client.Factory
{
    /// <summary>
    /// 
    /// </summary>
    public class CardDavClientFactory : ICardDavClientFactory
    {
        readonly IHttpClientFactory httpClientFactory;
        readonly ILogger<CardDavClient> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CardDavClientFactory(IHttpClientFactory httpClientFactory, ILogger<CardDavClient> logger)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region ICardDavClientFactory

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public CardDavClient CreateClient(CardDavOptions options)
        {
            var client = httpClientFactory.CreateClient("carddav");

            return new CardDavClient(client, logger, options);
        }

        #endregion
    }
    /// <summary>
    /// Creates a CardDav clients
    /// </summary>
    public interface ICardDavClientFactory
    {
        /// <summary>
        /// Creates a <see cref="CardDavClient"/> with options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        CardDavClient CreateClient(CardDavOptions options);
    }
}
