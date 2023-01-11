using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.CardDav.Server.Tests;
using BrandUp.Extensions.Migrations;
using MongoDB.Driver;

namespace BrandUp.CardDav.Server.Controllers.Tests._migration
{
    [Setup]
    public class TestUserMigration : IMigrationHandler
    {
        readonly AppDocumentContext context;

        public TestUserMigration(AppDocumentContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region IMigrationHandler members

        public Task DownAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task UpAsync(CancellationToken cancellationToken = default)
        {
            await CreateTestUserAsync(cancellationToken);
        }

        #endregion

        #region Helpers

        private async Task CreateTestUserAsync(CancellationToken cancellationToken)
        {
            var bookGuid = Guid.NewGuid();
            var userGuid = Guid.NewGuid();

            await context.Users.InsertOneAsync(new()
            {
                Id = userGuid,
                Name = "User",
                Password = "Password",
                CTag = DateTime.UtcNow.ToString()
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.AddressBooks.InsertOneAsync(new()
            {
                Id = bookGuid,
                Name = "Default",
                UserId = userGuid,
                CTag = DateTime.UtcNow.ToString()
            },
               new InsertOneOptions { BypassDocumentValidation = false },
               cancellationToken);

            await context.Contacts.InsertOneAsync(new()
            {
                AddressBookId = bookGuid,
                Name = "first",
                ETag = "cxzvm",
                Id = Guid.NewGuid(),
                RawVCard = TestVCards.VCard1String
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.Contacts.InsertOneAsync(new()
            {
                AddressBookId = bookGuid,
                Name = "second",
                ETag = "cxzvm",
                Id = Guid.NewGuid(),
                RawVCard = TestVCards.VCard2String
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.Contacts.InsertOneAsync(new()
            {
                AddressBookId = bookGuid,
                Name = "third",
                ETag = "cxzvm",
                Id = Guid.NewGuid(),
                RawVCard = TestVCards.VCard3String
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);
        }

        #endregion
    }
}
