using BrandUp.CardDav.Server.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    public interface IContactRepository
    {
        public IQueryable<IContactDocument> Contacts { get; }
        public Task CreateAsync(string name, Guid bookId, string vCard, CancellationToken cancellationToken);
        public Task<bool> UpdateAsync(IContactDocument document, string eTag, CancellationToken cancellationToken);
        public Task<IContactDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<IEnumerable<IContactDocument>> FindAllContactsByBookIdAsync(Guid collectionId, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(IContactDocument document, CancellationToken cancellationToken);
    }
}
