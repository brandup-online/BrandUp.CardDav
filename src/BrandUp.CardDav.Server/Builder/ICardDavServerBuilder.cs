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
        public CardDavServerBuilder(IServiceCollection services, CardDavServerOptions options)
        {
            Services = services ?? throw new ArgumentNullException();
            Options = options ?? throw new ArgumentNullException();


            Services.AddSingleton<ICardDavServerBuilder, CardDavServerBuilder>((sp) => this);
            AddServices();
        }


        #region ICardDavServerBuilder members

        /// <summary>
        /// 
        /// </summary>
        public IServiceCollection Services { get; }
        /// <summary>
        /// 
        /// </summary>
        public CardDavServerOptions Options { get; }

        #endregion

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

        CardDavServerOptions Options { get; }
    }
}
