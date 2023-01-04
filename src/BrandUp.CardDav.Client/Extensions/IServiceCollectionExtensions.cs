using BrandUp.CardDav.Client.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Client
{
    /// <summary>
    /// 
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a ICardDavClientFactory to service collection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
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
