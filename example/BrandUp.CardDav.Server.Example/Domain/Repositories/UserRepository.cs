using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Domain.Documents;
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

        public Task CreateAsync(string name, CancellationToken cancellationToken)
        {
            UserDocument document = new();
            document.SetDorCreatiion(name);

            return context.Users.InsertOneAsync(document, new() { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task<bool> DeleteAsync(IUserDocument document, CancellationToken cancellationToken)
        {
            var result = await context.Users.DeleteOneAsync(d => d.Id == document.Id, cancellationToken);

            return result.DeletedCount == 1;
        }

        public async Task<IUserDocument> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var cursor = await context.Users.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IUserDocument> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            var cursor = await context.Users.FindAsync(u => u.Name == name, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(IUserDocument document, CancellationToken cancellationToken)
        {
            var result = await context.Users.ReplaceOneAsync(d => d.Id == document.Id, (UserDocument)document, cancellationToken: cancellationToken);

            return result.ModifiedCount == 1;
        }

        #endregion
    }
}
