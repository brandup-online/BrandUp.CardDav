using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.Extensions.Migrations;
using MongoDB.Driver;

namespace BrandUp.CardDav.Server.Controllers.Tests._migration
{
    [Setup]
    public class TestUserMigration : IMigrationHandler
    {
        readonly AppDocumentContext context;

        const string vCard1 = "BEGIN:VCARD\r\n" +
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

        const string vCard2 = "BEGIN:VCARD\r\n" +
                            "VERSION:3.0\r\n" +
                            "N:Die;Jahn;;;\r\n" +
                            "FN:Jahn Die\r\n" +
                            "ORG:Example.net Inc.;\r\n" +
                            "TITLE:Imaginary test person\r\n" +
                            "EMAIL;type=WORK;type=INTERNET;type=pref:jahnDie@example.org\r\n" +
                            "TEL;type=WORK;type=pref:+1 627 555 1212\r\n" +
                            "TEL;type=WORK:+1 (617) 535-1234\r\n" +
                            "TEL;type=CELL:+1 781 555 1242\r\n" +
                            "TEL;type=HOME:+1 202 555 1211\r\n" +
                            "END:VCARD\r\n";

        const string vCard3 = "BEGIN:VCARD\r\n" +
                            "VERSION:3.0\r\n" +
                            "N:Sha;Di;;;\r\n" +
                            "FN:Di Sha\r\n" +
                            "ORG:Example.com Inc.;\r\n" +
                            "TITLE:Imaginary test person\r\n" +
                            "EMAIL;type=WORK;type=INTERNET;type=pref:milo@example.org\r\n" +
                            "TEL;type=WORK;type=pref:+1 232 555 1212\r\n" +
                            "TEL;type=WORK:+1 (617) 666-1234\r\n" +
                            "TEL;type=CELL:+1 781 777 1212\r\n" +
                            "TEL;type=HOME:+1 202 113 2112\r\n" +
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

            await context.Users.InsertOneAsync(new()
            {
                Id = userGuid,
                Name = "User"
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
                RawVCard = vCard1
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.Contacts.InsertOneAsync(new()
            {
                AddressBookId = bookGuid,
                Name = "second",
                ETag = "cxzvm",
                Id = Guid.NewGuid(),
                RawVCard = vCard2
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.Contacts.InsertOneAsync(new()
            {
                AddressBookId = bookGuid,
                Name = "third",
                ETag = "cxzvm",
                Id = Guid.NewGuid(),
                RawVCard = vCard3
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);
        }

        #endregion
    }
}
