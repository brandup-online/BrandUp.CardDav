using BrandUp.CardDav.Transport.Handling;
using BrandUp.CardDav.Transport.Handling.Context;
using BrandUp.CardDav.Transport.Server.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Server.Builder
{
    /// <summary>
    /// Create CardDav server
    /// </summary>
    public class CardDavServerBuilder : ICardDavServerBuilder
    {
        /// <summary>
        /// Construoctor
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CardDavServerBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException();

            AddServices();
        }

        private void AddServices()
        {
            Services.AddHttpContextAccessor();
            Services.AddScoped<IHandlerContext, HandlerContext>();

            Services.AddScoped<AddressDataHandler>();
            Services.AddScoped<CtagHandler>();
            Services.AddScoped<CurrentUserPrincipalHandler>();
            Services.AddScoped<EtagHandler>();
            Services.AddScoped<PrincipalUrlHandler>();
            Services.AddScoped<ResourcetypeHandler>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IServiceCollection Services { get; }
    }

    /// <summary>
    /// Create CardDav server
    /// </summary>
    public interface ICardDavServerBuilder
    {
        /// <summary>
        /// Service collection
        /// </summary>
        IServiceCollection Services { get; }
    }
}
