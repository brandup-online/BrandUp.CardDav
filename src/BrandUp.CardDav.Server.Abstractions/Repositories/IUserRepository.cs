using BrandUp.CardDav.Server.Abstractions.Additional;
using BrandUp.CardDav.Server.Abstractions.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    /// <summary>
    /// Provides methods for access to database for users.
    /// </summary>
    public interface IUserRepository
    {

        /// <summary>
        /// Creates a new contact.
        /// </summary>
        /// <param name="name">User document.</param>
        /// <param name="password">User document.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task CreateAsync(string name, string password, CancellationToken cancellationToken);

        /// <summary>
        /// Updates user
        /// </summary>
        /// <param name="document">Updated document</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(User document, CancellationToken cancellationToken);

        /// <summary>
        /// Finding user by id
        /// </summary>
        /// <param name="id">User identifier.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<User> FindByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Finding user by name.
        /// </summary>
        /// <param name="name">Contact name.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<User> FindByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<UserCredentials> FindCredentialsByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CTag> GetCTagAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="document">User</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(User document, CancellationToken cancellationToken);
    }
}
