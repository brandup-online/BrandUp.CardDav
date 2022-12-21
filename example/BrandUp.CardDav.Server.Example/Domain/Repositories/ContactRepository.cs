using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Domain.Documents;
using BrandUp.CardDav.Server.Repositories;
using MongoDB.Driver;

namespace BrandUp.CardDav.Server.Example.Domain.Repositories
{
    public class ContactRepository : IContactRepository
    {
        readonly AppDocumentContext context;

        public ContactRepository(AppDocumentContext context)
        {
            this.context = context ?? throw new ArgumentNullException();
        }

        #region IContactRepository member

        public IQueryable<IContactDocument> Contacts => context.Contacts.AsQueryable();

        public Task CreateAsync(string name, Guid bookId, string vCard, CancellationToken cancellationToken)
        {
            ContactDocument document = new();
            document.SetForCreation(name, bookId, vCard);

            return context.Contacts.InsertOneAsync(document, new() { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task<bool> DeleteAsync(IContactDocument document, CancellationToken cancellationToken)
        {
            var result = await context.Contacts.DeleteOneAsync(d => d.Id == document.Id, cancellationToken);

            return result.DeletedCount == 1;
        }

        public async Task<IEnumerable<IContactDocument>> FindAllContactsByBookIdAsync(Guid collectionId, CancellationToken cancellationToken)
        {
            var cursor = await context.Contacts.FindAsync(u => u.AddressBookId == collectionId, cancellationToken: cancellationToken);

            return cursor.ToList();
        }

        public async Task<IContactDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var cursor = await context.Contacts.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(IContactDocument document, string eTag, CancellationToken cancellationToken)
        {
            var result = await context.Contacts.ReplaceOneAsync(d => d.Id == document.Id, (ContactDocument)document, cancellationToken: cancellationToken);

            return result.ModifiedCount == 1;
        }

        #endregion
    }
}
