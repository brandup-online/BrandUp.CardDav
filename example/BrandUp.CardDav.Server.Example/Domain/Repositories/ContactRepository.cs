using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Example.Domain.Context;
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

        public Task CreateAsync(IContactDocument document, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(IContactDocument document, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IContactDocument>> FindAllContactsByCollectionIdAsync(Guid collectionId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IContactDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(IContactDocument document, string eTag, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
