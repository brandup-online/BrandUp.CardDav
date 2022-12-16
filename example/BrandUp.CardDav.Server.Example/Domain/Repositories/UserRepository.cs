using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Repositories;
using MongoDB.Driver;

namespace BrandUp.CardDav.Server.Example.Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        readonly AppDocumentContext context;

        public UserRepository(AppDocumentContext context)
        {
            this.context = context ?? throw new ArgumentNullException();
        }

        #region IUserRepository members

        public IQueryable<IUserDocument> Users => context.Users.AsQueryable();

        public Task CreateAsync(IUserDocument document, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(IUserDocument document, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IUserDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IUserDocument> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(IUserDocument document, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
