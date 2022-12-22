using BrandUp.CardDav.Server.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Server
{
    public static class IServiceCollectionExtension
    {

        public static ICardDavServerBuilder AddCradDavServer(this IServiceCollection services)
        {
            return new CardDavServerBuilder(services);
        }
    }
}
