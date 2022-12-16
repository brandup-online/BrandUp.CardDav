using BrandUp.CardDav.Server.Builder;
using BrandUp.CardDav.Server.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Server
{
    public static class ICardDavServerBuilderExtension
    {
        public static ICardDavServerBuilder AddUsers<TRepository>(this ICardDavServerBuilder builder) where TRepository : class, IUserRepository
        {
            builder.Services.AddSingleton<IUserRepository, TRepository>();

            return builder;
        }

        public static ICardDavServerBuilder AddAddressBooks<TRepository>(this ICardDavServerBuilder builder) where TRepository : class, IAddressBookRepository
        {
            builder.Services.AddSingleton<IAddressBookRepository, TRepository>();

            return builder;
        }

        public static ICardDavServerBuilder AddContacts<TRepository>(this ICardDavServerBuilder builder) where TRepository : class, IContactRepository
        {
            builder.Services.AddSingleton<IContactRepository, TRepository>();

            return builder;
        }
    }
}
