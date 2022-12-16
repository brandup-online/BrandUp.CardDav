using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.Extensions.Migrations;
using MongoDB.Driver;

namespace BrandUp.CardDav.Server.Controllers.Tests._migration
{
    public class TestUserMigration : IMigrationHandler
    {
        readonly AppDocumentContext context;

        const string vCard = "BEGIN:VCARD\r\n" +
            "VERSION:3.0\r\n" +
            "N:Doe;John;;;\r\n" +
            "FN:John Doe\r\n" +
            "ORG:Example.com Inc.;\r\n" +
            "TITLE:Imaginary test person\r\n" +
            "EMAIL;type=WORK;type=INTERNET;type=pref:johnDoe@example.org\r\n" +
            "TEL;type=WORK;type=pref:+1 617 555 1212\r\n" +
            "TEL;type=WORK:+1 (617) 555-1234\r\n" +
            "TEL;type=CELL:+1 781 555 1212\r\n" +
            "TEL;type=HOME:+1 202 555 1212\r\n" +
            "END:VCARD\r\n";

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

            await context.AddressBooks.InsertOneAsync(new()
            {
                Id = bookGuid,
                UserId = userGuid,
                ETag = DateTime.UtcNow.ToString()
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.Users.InsertOneAsync(new()
            {
                Id = userGuid,
                Name = "User",
                AddressBooks = new List<Guid> { bookGuid }
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.Contacts.InsertOneAsync(new()
            {
                AddressBookId = bookGuid,
                Id = Guid.NewGuid(),
                VCard = vCard
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

        }

        #endregion
    }
}
