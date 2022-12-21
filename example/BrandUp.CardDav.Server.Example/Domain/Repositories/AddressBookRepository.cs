using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Domain.Documents;
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

        public IQueryable<IAddressBookDocument> AddressBooks => context.AddressBooks.AsQueryable();

        public Task CreateAsync(string addressBook, Guid userId, CancellationToken cancellationToken)
        {
            var document = new AddressBookDocument();
            document.SetForCreation(addressBook, userId);
            return context.AddressBooks.InsertOneAsync(document, new() { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task<bool> DeleteAsync(IAddressBookDocument document, CancellationToken cancellationToken)
        {
            var result = await context.AddressBooks.DeleteOneAsync(d => d.Id == document.Id, cancellationToken);

            return result.DeletedCount == 1;
        }

        public async Task<IAddressBookDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var cursor = await context.AddressBooks.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<IAddressBookDocument> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            var cursor = await context.AddressBooks.FindAsync(u => u.Name == name, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<IAddressBookDocument>> FindCollectionsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var cursor = await context.AddressBooks.FindAsync(u => u.UserId == userId, cancellationToken: cancellationToken);

            return cursor.ToList();
        }

        public async Task<bool> UpdateAsync(IAddressBookDocument document, string eTag, CancellationToken cancellationToken)
        {
            var result = await context.AddressBooks.ReplaceOneAsync(d => d.Id == document.Id, (AddressBookDocument)document, cancellationToken: cancellationToken);

            return result.ModifiedCount == 1;
        }

        #endregion
    }
}
