using BrandUp.CardDav.Server.Abstractions.Additional;
using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Example.Domain.Documents;
using BrandUp.CardDav.Server.Example.Mapping;
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

        public async Task<UserDocument> FindDocumentByNameAsync(string name, CancellationToken cancellationToken)
        {
            var cursor = await context.Users.FindAsync(u => u.Name == name, cancellationToken: cancellationToken);

            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }

        #region IUserRepository members

        public Task CreateAsync(string name, string password, CancellationToken cancellationToken)
        {
            var user = new UserDocument();

            user.SetForCreation(name, password);

            return context.Users.InsertOneAsync(new Documents.UserDocument { Id = user.Id, Name = user.Name, }, new() { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Abstractions.Documents.User document, CancellationToken cancellationToken)
        {
            var result = await context.Users.DeleteOneAsync(d => d.Id == document.Id, cancellationToken);

            return result.DeletedCount == 1;
        }

        public async Task<Abstractions.Documents.User> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var cursor = await context.Users.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);

            return (await cursor.FirstOrDefaultAsync(cancellationToken))?.ToUser();
        }

        public async Task<Abstractions.Documents.User> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            var cursor = await context.Users.FindAsync(u => u.Name == name, cancellationToken: cancellationToken);

            return (await cursor.FirstOrDefaultAsync(cancellationToken))?.ToUser();
        }

        public async Task<CTag> GetCTagAsync(Guid id, CancellationToken cancellationToken)
        {
            var cursor = await context.Users.FindAsync(u => u.Id == id, cancellationToken: cancellationToken);
            var user = await cursor.FirstOrDefaultAsync(cancellationToken);

            return new CTag { Ctag = user.CTag };
        }

        public async Task<bool> UpdateAsync(Abstractions.Documents.User user, CancellationToken cancellationToken)
        {
            var cursor = await context.Users.FindAsync(u => u.Name == user.Name && u.Id == user.Id, cancellationToken: cancellationToken);

            var document = await cursor.FirstOrDefaultAsync(cancellationToken);

            document.Name = user.Name;

            var result = await context.Users.ReplaceOneAsync(d => d.Id == document.Id, document, cancellationToken: cancellationToken);

            return result.ModifiedCount == 1;
        }

        #endregion
    }
}
