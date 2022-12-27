using BrandUp.CardDav.Client.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Client
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCardDavClient(this IServiceCollection services)
        {
            services.AddHttpClient("carddav").ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler()
                    {
                        AllowAutoRedirect = false,
                    });

            services.AddScoped<ICardDavClientFactory, CardDavClientFactory>();

            return services;
        }
    }
}
