using BrandUp.Carddav.Client.Client;
using BrandUp.Carddav.Client.Factory;
using BrandUp.Carddav.Client.Options;

namespace BrandUp.Carddav.Client.Extensions
{
    public static class ICardDavClientFactoryExtensions
    {
        public static CardDavClient CreateClientWithCredentials(this ICardDavClientFactory factory, string url, string login, string password)
        => factory.CreateClient(new CardDavCredentialsOptions { BaseUrl = url, Login = login, Password = password });
        public static CardDavClient CreateClientWithAccessToken(this ICardDavClientFactory factory, string url, string accessToken)
        => factory.CreateClient(new CardDavOAuthOptions { BaseUrl = url, AccessToken = accessToken });
    }
}
