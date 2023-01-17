using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Domain.Documents;
using BrandUp.CardDav.Server.Example.Mapping;
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

        public async Task CreateAsync(string name, Guid bookId, string vCard, CancellationToken cancellationToken)
        {
            Documents.ContactDocument document = new();
            document.SetForCreation(name, bookId, vCard);

            await context.Contacts.InsertOneAsync(document, new() { BypassDocumentValidation = false }, cancellationToken);
            await UpdateCTagAsync(bookId, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Abstractions.Documents.Contact document, CancellationToken cancellationToken)
        {
            var result = await context.Contacts.DeleteOneAsync(d => d.Id == document.Id, cancellationToken);

            await UpdateCTagAsync(document.AddressBookId, cancellationToken);
            return result.DeletedCount == 1;
        }

        public async Task<IEnumerable<Abstractions.Documents.Contact>> FindAllContactsByBookIdAsync(Guid collectionId, CancellationToken cancellationToken)
        {
            var cursor = await context.Contacts.FindAsync(u => u.AddressBookId == collectionId, cancellationToken: cancellationToken);

            return (await cursor.ToListAsync(cancellationToken)).Select(_ => _.ToContact());
        }

        public async Task<Abstractions.Documents.Contact> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var cursor = await context.Contacts.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);

            return (await cursor.FirstOrDefaultAsync(cancellationToken))?.ToContact();
        }

        public async Task<Abstractions.Documents.Contact> FindByNameAsync(string name, Guid bookId, CancellationToken cancellationToken)
        {
            var cursor = await context.Contacts.FindAsync(u => u.Name == name && u.AddressBookId == bookId, cancellationToken: cancellationToken);

            return (await cursor.FirstOrDefaultAsync(cancellationToken))?.ToContact();
        }

        public async Task<bool> UpdateAsync(Abstractions.Documents.Contact contact, string eTag, CancellationToken cancellationToken)
        {
            var cursor = await context.Contacts.FindAsync(u => u.Id == contact.Id, cancellationToken: cancellationToken);

            var document = await cursor.FirstOrDefaultAsync(cancellationToken);

            document.ETag = contact.ETag;
            document.RawVCard = contact.RawVCard;

            var result = await context.Contacts.ReplaceOneAsync(d => d.Id == document.Id, document, cancellationToken: cancellationToken);

            await UpdateCTagAsync(document.AddressBookId, cancellationToken);

            return result.ModifiedCount == 1;
        }

        #endregion

        #region Helpers

        async Task UpdateCTagAsync(Guid bookId, CancellationToken cancellationToken)
        {
            var builder = new FilterDefinitionBuilder<AddressBookDocument>();
            var fillter = builder.Eq(b => b.Id, bookId);

            var updateBuilder = new UpdateDefinitionBuilder<AddressBookDocument>();
            var update = updateBuilder.Set(b => b.CTag, DateTime.UtcNow.ToString());

            await context.AddressBooks.UpdateOneAsync(fillter, update, cancellationToken: cancellationToken);
        }

        #endregion
    }
}
