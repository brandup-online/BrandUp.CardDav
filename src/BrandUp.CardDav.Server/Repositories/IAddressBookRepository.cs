using BrandUp.CardDav.Server.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    public interface IAddressBookRepository
    {
        public IQueryable<IAddressBookDocument> AddressBooks { get; }
        public Task CreateAsync(string name, Guid userId, CancellationToken cancellationToken);
        public Task<bool> UpdateAsync(IAddressBookDocument document, string eTag, CancellationToken cancellationToken);
        public Task<IAddressBookDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<IAddressBookDocument> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken);
        public Task<IEnumerable<IAddressBookDocument>> FindCollectionsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(IAddressBookDocument document, CancellationToken cancellationToken);
    }
}
