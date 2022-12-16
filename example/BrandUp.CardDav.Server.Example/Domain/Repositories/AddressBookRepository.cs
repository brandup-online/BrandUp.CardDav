using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Example.Domain.Context;
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

        public Task CreateAsync(IAddressBookDocument document, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(IAddressBookDocument document, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IAddressBookDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IAddressBookDocument>> FindCollectionsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(IAddressBookDocument document, string eTag, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
