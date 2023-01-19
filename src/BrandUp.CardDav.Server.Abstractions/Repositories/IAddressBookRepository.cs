using BrandUp.CardDav.Server.Abstractions.Additional;
using BrandUp.CardDav.Server.Abstractions.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    /// <summary>
    /// Provides methods for access to database for address book
    /// </summary>
    public interface IAddressBookRepository
    {

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
        public Task<bool> UpdateAsync(AddressBook document, string eTag, CancellationToken cancellationToken);

        /// <summary>
        /// Finding address book by id
        /// </summary>
        /// <param name="id">Address book identifier</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AddressBook> FindByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Finding address book by name
        /// </summary>
        /// <param name="name">Address book name</param>
        /// <param name="userId">Identifier of user who owns the book</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AddressBook> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Finds all address book for user
        /// </summary>
        /// <param name="userId">Identifier of user who owns the book</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IEnumerable<AddressBook>> FindCollectionsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CTag> GetCTagAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a addressbook
        /// </summary>
        /// <param name="document">Address book</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(AddressBook document, CancellationToken cancellationToken);
    }
}
