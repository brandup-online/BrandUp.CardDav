using BrandUp.CardDav.Server.Documents;

namespace BrandUp.CardDav.Server.Repositories
{
    public interface IUserRepository
    {
        public IQueryable<IUserDocument> Users { get; }
        public Task CreateAsync(string name, CancellationToken cancellationToken);
        public Task<bool> UpdateAsync(IUserDocument document, CancellationToken cancellationToken);
        public Task<IUserDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<IUserDocument> FindByNameAsync(string name, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(IUserDocument document, CancellationToken cancellationToken);
    }
}
