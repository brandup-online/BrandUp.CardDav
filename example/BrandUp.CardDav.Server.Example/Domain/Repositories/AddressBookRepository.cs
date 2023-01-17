using BrandUp.CardDav.Server.Abstractions.Additional;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Mapping;
using BrandUp.CardDav.Server.Repositories;
using MongoDB.Driver;

namespace BrandUp.CardDav.Server.Example.Domain.Repositories
{
    public class AddressBookRepository : IAddressBookRepository
    {
        readonly AppDocumentContext context;

        public AddressBookRepository(AppDocumentContext context)
        {
            this.context = context ?? throw new ArgumentNullException();
        }

        #region IAddressBookRepository members

        public Task CreateAsync(string addressBook, Guid userId, CancellationToken cancellationToken)
        {
            var document = new Documents.AddressBookDocument();
            document.SetForCreation(addressBook, userId);
            return context.AddressBooks.InsertOneAsync(document, new() { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Abstractions.Documents.AddressBook document, CancellationToken cancellationToken)
        {
            var result = await context.AddressBooks.DeleteOneAsync(d => d.Id == document.Id, cancellationToken);

            return result.DeletedCount == 1;
        }

        public async Task<Abstractions.Documents.AddressBook> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var cursor = await context.AddressBooks.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);

            return (await cursor.FirstOrDefaultAsync())?.ToAddressBook();
        }

        public async Task<Abstractions.Documents.AddressBook> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken)
        {
            var cursor = await context.AddressBooks.FindAsync(u => u.Name == name && u.UserId == userId, cancellationToken: cancellationToken);

            return (await cursor.FirstOrDefaultAsync(cancellationToken))?.ToAddressBook();
        }

        public async Task<IEnumerable<Abstractions.Documents.AddressBook>> FindCollectionsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var cursor = await context.AddressBooks.FindAsync(u => u.UserId == userId, cancellationToken: cancellationToken);

            return (await cursor.ToListAsync(cancellationToken)).Select(_ => _.ToAddressBook());
        }

        public async Task<CTag> GetCTagAsync(Guid id, CancellationToken cancellationToken)
        {
            var cursor = await context.AddressBooks.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);
            var book = await cursor.FirstOrDefaultAsync(cancellationToken);

            return new CTag { Ctag = book.CTag };
        }

        public async Task<bool> UpdateAsync(Abstractions.Documents.AddressBook book, string eTag, CancellationToken cancellationToken)
        {
            var cursor = await context.AddressBooks.FindAsync(u => u.Name == book.Name && u.Id == book.Id, cancellationToken: cancellationToken);

            var document = await cursor.FirstOrDefaultAsync(cancellationToken);

            document.UserId = book.UserId;
            document.Name = book.Name;

            var result = await context.AddressBooks.ReplaceOneAsync(d => d.Id == document.Id, document, cancellationToken: cancellationToken);

            return result.ModifiedCount == 1;
        }

        #endregion
    }
}
