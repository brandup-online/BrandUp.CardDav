using BrandUp.Carddav.Client.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.Carddav.Client.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCardDavClient(this IServiceCollection services)
        {
            services.AddHttpClient("carddav").ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler()
                    {
                        AllowAutoRedirect = true,
                    });


            services.AddScoped<ICardDavClientFactory, CardDavClientFactory>();

            return services;
        }
    }
}
