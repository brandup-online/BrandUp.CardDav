using BrandUp.Carddav.Client.Builders;
using BrandUp.Carddav.Client.Extensions;
using BrandUp.Carddav.Client.Factory;
using BrandUp.Carddav.Client.Models;
using BrandUp.Carddav.Client.Models.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace BrandUp.Carddav.Client.Test
{
    public class CardDavClientTest : IAsyncLifetime
    {
        readonly ITestOutputHelper output;

        private ICardDavClientFactory cardDavClientFactory;

        private ServiceProvider provider;
        private IServiceScope scope;

        private string userName;
        private string password;

        public CardDavClientTest(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));

            var services = new ServiceCollection();

            services.AddHttpClient();
            services.AddLogging();

            services.AddScoped<ICardDavClientFactory, CardDavClientFactory>();

            provider = services.BuildServiceProvider();
            scope = provider.CreateScope();

            var configuration = new ConfigurationBuilder().AddUserSecrets(typeof(CardDavClientTest).Assembly).Build();

            userName = configuration.GetSection("Cred:UserName").Get<string>();
            password = configuration.GetSection("Cred:Password").Get<string>();

            cardDavClientFactory = scope.ServiceProvider.GetRequiredService<ICardDavClientFactory>();
        }

        #region IAsyncLifetime members

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await provider.DisposeAsync();
            scope.Dispose();
        }

        #endregion

        [Fact]
        public async Task Success_Yandex_Basic()
        {
            var client = cardDavClientFactory.CreateClientWithCredentials("https://carddav.yandex.ru/", userName, password);

            var response = await client.OptionsAsync(CancellationToken.None);

            Assert.True(response.IsSuccess);

            response = await client.PropfindAsync($"/addressbook/{userName}/", new CarddavRequest { Depth = "1" }, CancellationToken.None);

            Assert.True(response.IsSuccess);
            Assert.Equal(2, response.addressBooks.Count);

            response = await client.PropfindAsync(response.addressBooks[1].Endpoint, new CarddavRequest { Depth = "1" }, CancellationToken.None);

            Assert.True(response.IsSuccess);
            Assert.Equal(1, response.addressBooks.Count);
            Assert.Equal(5, response.vCardLinks.Count);

            response = await client.GetAsync(response.vCardLinks[0].Endpoint, CancellationToken.None);
            Assert.True(response.IsSuccess);
            Assert.Single(response.vCards);
        }

        [Fact]
        public async Task Success_Yandex_CRUD()
        {
            var client = cardDavClientFactory.CreateClientWithCredentials("https://carddav.yandex.ru/", userName, password);

            var response = await client.OptionsAsync(CancellationToken.None);

            Assert.True(response.IsSuccess);

            var testPerson = "BEGIN:VCARD\r\nVERSION:3.0\r\nN:Doe;John;;;\r\nFN:John Doe\r\nORG:Example.com Inc.;\r\nTITLE:Imaginary test person\r\nEMAIL;type=INTERNET;type=WORK;type=pref:johnDoe@example.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nTEL;type=WORK:+1 (617) 555-1234\r\nTEL;type=CELL:+1 781 555 1212\r\nTEL;type=HOME:+1 202 555 1212\r\nEND:VCARD\r\n";

            var vCard = VCardBuilder.Create(testPerson).AddUId("2312133421324668575897435").Build();

            response = await client.AddContactAsync($"/addressbook/{userName}/addressbook/new", vCard, CancellationToken.None);
            Assert.True(response.IsSuccess);

            response = await client.GetAsync($"/addressbook/{userName}/addressbook/new", CancellationToken.None);
            Assert.True(response.IsSuccess);

            var responseVCard = response.vCards[0];
            Assert.Equal(vCard.Name.FamilyNames, responseVCard.Name.FamilyNames);
            Assert.Equal(vCard.Name.GivenNames, responseVCard.Name.GivenNames);
            Assert.Equal(vCard.Name.AdditionalNames, responseVCard.Name.AdditionalNames);
            Assert.Equal(vCard.Name.HonorificPrefixes, responseVCard.Name.HonorificPrefixes);
            Assert.Equal(vCard.Name.HonorificSuffixes, responseVCard.Name.HonorificSuffixes);
            Assert.Equal(vCard.FullName, responseVCard.FullName);
            Assert.Equal(vCard.Phones, responseVCard.Phones, new PhonesEqualityComparer());
            Assert.Equal(vCard.Emails, responseVCard.Emails, new EmailsEqualityComparer());

            response = await client.DeleteContactAsync($"/addressbook/{userName}/addressbook/new", CancellationToken.None);
            Assert.True(response.IsSuccess);

            response = await client.PropfindAsync($"/addressbook/{userName}/addressbook", new CarddavRequest { Depth = "1" }, CancellationToken.None);

            Assert.True(response.IsSuccess);
            Assert.Equal(5, response.vCardLinks.Count);
        }

        public class PhonesEqualityComparer : IEqualityComparer<VCardPhone>
        {
            public bool Equals(VCardPhone x, VCardPhone y)
            {
                return x.Phone == y.Phone;
            }

            public int GetHashCode([DisallowNull] VCardPhone obj)
            {
                return obj.Kind.GetHashCode() + obj.Phone.GetHashCode();
            }
        }

        public class EmailsEqualityComparer : IEqualityComparer<VCardEmail>
        {
            public bool Equals(VCardEmail x, VCardEmail y)
            {
                return x.Email == y.Email;
            }

            public int GetHashCode([DisallowNull] VCardEmail obj)
            {
                return obj.Kind.GetHashCode() + obj.Email.GetHashCode();
            }
        }

        #region I think this is will useful later
        //propfind
        //var content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
        //"<d:propfind xmlns:d=\"DAV:\" xmlns:cs=\"http://calendarserver.org/ns/\">\r\n  " +
        //"<d:prop>\r\n    " +
        //" <d:displayname />\r\n    " +
        //" <cs:getctag />\r\n " +
        //" <d:sync-token />\r\n" +
        //" </d:prop>\r\n</d:propfind>";

        //report
        //        var content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
        //"  <C:addressbook-query xmlns:D=\"DAV:\"\n          " +
        //"           xmlns:C=\"urn:ietf:params:xml:ns:carddav\">\n   " +
        //"  <D:prop>\n  " +
        //"     <D:getetag/>\n     " +
        //"  <C:address-data>\n      " +
        //"     </C:address-data>\n " +
        //"    </D:prop>\n    " +
        //"<C:filter>\r\n    <C:prop-filter name=\"FN\">\r\n    </C:prop-filter>    \r\n</C:filter>" +
        //"</C:addressbook-query>";
        #endregion
    }
}