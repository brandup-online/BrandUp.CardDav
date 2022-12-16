using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Server.Builder
{
    public class CardDavServerBuilder : ICardDavServerBuilder
    {
        public CardDavServerBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException();
        }

        public IServiceCollection Services { get; }
    }

    public interface ICardDavServerBuilder
    {
        IServiceCollection Services { get; }
    }
}
