using BrandUp.CardDav.Server.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Server
{
    /// <summary>
    /// 
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Creates server builder
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ICardDavServerBuilder AddCradDavServer(this IServiceCollection services, Action<CardDavServerOptions> action)
        {
            var options = new CardDavServerOptions();
            action(options);

            return new CardDavServerBuilder(services, options);
        }
    }
}
