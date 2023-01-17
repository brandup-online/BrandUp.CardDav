using BrandUp.CardDav.Server.Abstractions.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    /// <summary>
    /// Provides methods for access to database for contacts.
    /// </summary>
    public interface IContactRepository
    {
        /// <summary>
        /// Creates a new contact.
        /// </summary>
        /// <param name="name">Contact name.</param>
        /// <param name="bookId">The address book in which there is this contact.</param>
        /// <param name="vCard">VCard contact in string.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task CreateAsync(string name, Guid bookId, string vCard, CancellationToken cancellationToken);

        /// <summary>
        /// Updates Contact
        /// </summary>
        /// <param name="document">Updated document</param>
        /// <param name="eTag">Entity tag</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(Contact document, string eTag, CancellationToken cancellationToken);

        /// <summary>
        /// Finding contact by id
        /// </summary>
        /// <param name="id">Contact identifier.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Contact> FindByIdAsync(Guid id, CancellationToken cancellationToken);


        /// <summary>
        /// Finding contact by name.
        /// </summary>
        /// <param name="name">Contact name.</param>
        /// <param name="bookId">The address book in which there is this contact.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Contact> FindByNameAsync(string name, Guid bookId, CancellationToken cancellationToken);

        /// <summary>
        /// Finds all contacts of address book.
        /// </summary>
        /// <param name="bookId">The address book in which there is this contact.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IEnumerable<Contact>> FindAllContactsByBookIdAsync(Guid bookId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a contact.
        /// </summary>
        /// <param name="document">Address book</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(Contact document, CancellationToken cancellationToken);
    }
}
