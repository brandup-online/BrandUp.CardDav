using BrandUp.CardDav.Server.Example.Domain.Context;
using BrandUp.Extensions.Migrations;
using MongoDB.Driver;

namespace BrandUp.CardDav.Server.Example._migrations
{
    public class UserMigration : IMigrationHandler
    {
        readonly AppDocumentContext context;

        public UserMigration(AppDocumentContext context)
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
            await CreateUserAsync(cancellationToken);
        }

        #endregion

        #region Helpers

        const string VCard1String = "BEGIN:VCARD\r\n" +
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

        const string VCard2String = "BEGIN:VCARD\r\n" +
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

        const string VCard3String = "BEGIN:VCARD\r\n" +
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

        private async Task CreateUserAsync(CancellationToken cancellationToken)
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
                Name = $"\"231312-31231232\"",
                ETag = "cxzvm",
                Id = Guid.NewGuid(),
                RawVCard = VCard1String
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.Contacts.InsertOneAsync(new()
            {
                AddressBookId = bookGuid,
                Name = "second",
                ETag = $"\"231232-56646543\"",
                Id = Guid.NewGuid(),
                RawVCard = VCard2String
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);

            await context.Contacts.InsertOneAsync(new()
            {
                AddressBookId = bookGuid,
                Name = "third",
                ETag = $"\"12657-213465787\"",
                Id = Guid.NewGuid(),
                RawVCard = VCard3String
            },
                new InsertOneOptions { BypassDocumentValidation = false },
                cancellationToken);
        }

        #endregion

    }
}
