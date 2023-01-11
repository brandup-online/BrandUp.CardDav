using BrandUp.CardDav.Server.Abstractions.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    /// <summary>
    /// Provides methods for access to database for users.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// List of all users in database.
        /// </summary>
        public IQueryable<IUserDocument> Users { get; }

        /// <summary>
        /// Creates a new contact.
        /// </summary>
        /// <param name="name">User name.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task CreateAsync(string name, CancellationToken cancellationToken);

        /// <summary>
        /// Updates user
        /// </summary>
        /// <param name="document">Updated document</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(IUserDocument document, CancellationToken cancellationToken);

        /// <summary>
        /// Finding user by id
        /// </summary>
        /// <param name="id">User identifier.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IUserDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Finding user by name.
        /// </summary>
        /// <param name="name">Contact name.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IUserDocument> FindByNameAsync(string name, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="document">User</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(IUserDocument document, CancellationToken cancellationToken);
    }
}
