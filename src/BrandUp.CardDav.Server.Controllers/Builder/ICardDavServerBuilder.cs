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
