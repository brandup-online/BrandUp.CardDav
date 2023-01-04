using BrandUp.CardDav.Client.Factory;
using BrandUp.CardDav.Client.Options;

namespace BrandUp.CardDav.Client
{
    /// <summary>
    /// Factory extensions
    /// </summary>
    public static class ICardDavClientFactoryExtensions
    {
        /// <summary>
        /// Creates a client, using login and password for authorization 
        /// </summary>
        /// <param name="factory">Client factory</param>
        /// <param name="url">Base URL</param>
        /// <param name="login">Server login</param>
        /// <param name="password">Server password</param>
        /// <returns></returns>
        public static ICardDavClient CreateClientWithCredentials(this ICardDavClientFactory factory, string url, string login, string password)
        => factory.CreateClient(new CardDavCredentialsOptions { BaseUrl = url, Login = login, Password = password });

        /// <summary>
        /// Creates a client, using access token for authorization
        /// </summary>
        /// <param name="factory">Client factory</param>
        /// <param name="url">Base URL</param>
        /// <param name="accessToken">Access token</param>
        /// <returns></returns>
        public static ICardDavClient CreateClientWithAccessToken(this ICardDavClientFactory factory, string url, string accessToken)
        => factory.CreateClient(new CardDavOAuthOptions { BaseUrl = url, AccessToken = accessToken });
    }
}
