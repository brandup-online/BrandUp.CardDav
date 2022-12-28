using BrandUp.CardDav.Client.Factory;
using BrandUp.CardDav.Client.Options;

namespace BrandUp.CardDav.Client
{
    public static class ICardDavClientFactoryExtensions
    {
        public static ICardDavClient CreateClientWithCredentials(this ICardDavClientFactory factory, string url, string login, string password)
        => factory.CreateClient(new CardDavCredentialsOptions { BaseUrl = url, Login = login, Password = password });
        public static ICardDavClient CreateClientWithAccessToken(this ICardDavClientFactory factory, string url, string accessToken)
        => factory.CreateClient(new CardDavOAuthOptions { BaseUrl = url, AccessToken = accessToken });
    }
}
