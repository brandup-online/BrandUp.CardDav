using BrandUp.CardDav.Server.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    public interface IContactRepository
    {
        public Task CreateAsync(IContactDocument document, CancellationToken cancellationToken);
        public Task<bool> UpdateAsync(IContactDocument document, string eTag, CancellationToken cancellationToken);
        public Task<IContactDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(IContactDocument document, CancellationToken cancellationToken);
    }
}
