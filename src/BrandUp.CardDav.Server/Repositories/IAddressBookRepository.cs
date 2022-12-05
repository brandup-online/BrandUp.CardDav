using BrandUp.CardDav.Server.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    public interface IAddressBookRepository
    {
        public Task CreateAsync(IAddressBookDocument document, CancellationToken cancellationToken);
        public Task<bool> UpdateAsync(IAddressBookDocument document, string eTag, CancellationToken cancellationToken);
        public Task<IAddressBookDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<IAddressBookDocument> FindByNameAsync(string name, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(IAddressBookDocument document, CancellationToken cancellationToken);
    }
}
