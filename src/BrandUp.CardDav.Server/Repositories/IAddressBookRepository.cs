using BrandUp.CardDav.Server.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    /// <summary>
    /// Provides methods for access to database for address book
    /// </summary>
    public interface IAddressBookRepository
    {
        /// <summary>
        /// List of all address books in database
        /// </summary>
        public IQueryable<IAddressBookDocument> AddressBooks { get; }

        /// <summary>
        /// Creates a new Address book
        /// </summary>
        /// <param name="name">Address book name</param>
        /// <param name="userId">The user who owns the book</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task CreateAsync(string name, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates Address book
        /// </summary>
        /// <param name="document">Updated document</param>
        /// <param name="eTag">Entity tag</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(IAddressBookDocument document, string eTag, CancellationToken cancellationToken);

        /// <summary>
        /// Finding address book by id
        /// </summary>
        /// <param name="id">Address book identifier</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IAddressBookDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Finding address book by name
        /// </summary>
        /// <param name="name">Address book name</param>
        /// <param name="userId">Identifier of user who owns the book</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IAddressBookDocument> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Finds all address book for user
        /// </summary>
        /// <param name="userId">Identifier of user who owns the book</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IEnumerable<IAddressBookDocument>> FindCollectionsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a addressbook
        /// </summary>
        /// <param name="document">Address book</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(IAddressBookDocument document, CancellationToken cancellationToken);
    }
}
