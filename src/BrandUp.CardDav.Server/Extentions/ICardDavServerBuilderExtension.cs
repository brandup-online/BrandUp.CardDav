using BrandUp.CardDav.Server.Builder;
using BrandUp.CardDav.Server.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BrandUp.CardDav.Server
{
    /// <summary>
    /// 
    /// </summary>
    public static class ICardDavServerBuilderExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ICardDavServerBuilder AddUsers<TRepository>(this ICardDavServerBuilder builder) where TRepository : class, IUserRepository
        {
            builder.Services.AddSingleton<IUserRepository, TRepository>();

            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ICardDavServerBuilder AddAddressBooks<TRepository>(this ICardDavServerBuilder builder) where TRepository : class, IAddressBookRepository
        {
            builder.Services.AddSingleton<IAddressBookRepository, TRepository>();

            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ICardDavServerBuilder AddContacts<TRepository>(this ICardDavServerBuilder builder) where TRepository : class, IContactRepository
        {
            builder.Services.AddSingleton<IContactRepository, TRepository>();

            return builder;
        }

        /// <summary>
        /// Adds all repositories for server to services
        /// </summary>
        /// <typeparam name="TUser"><see cref="IUserRepository"/> implementation</typeparam>
        /// <typeparam name="TAddressBook"><see cref="IAddressBookRepository"/> implementation</typeparam>
        /// <typeparam name="TContact"><see cref="IContactRepository"/> implementation</typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ICardDavServerBuilder AddRepositories<TUser, TAddressBook, TContact>(this ICardDavServerBuilder builder) where TUser : class, IUserRepository
                                                                                                                               where TAddressBook : class, IAddressBookRepository
                                                                                                                               where TContact : class, IContactRepository
        {
            builder.Services.AddSingleton<IUserRepository, TUser>();
            builder.Services.AddSingleton<IAddressBookRepository, TAddressBook>();
            builder.Services.AddSingleton<IContactRepository, TContact>();

            return builder;
        }
    }
}
