using BrandUp.Carddav.Client.Builders;
using BrandUp.Carddav.Client.Extensions;
using BrandUp.Carddav.Client.Factory;
using BrandUp.Carddav.Client.Helpers;
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
        private string token;
        private string gmail;

        public CardDavClientTest(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));

            var services = new ServiceCollection();

            services.AddHttpClient("carddav").ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler()
            {
                AllowAutoRedirect = true,
            });
            services.AddLogging();

            services.AddScoped<ICardDavClientFactory, CardDavClientFactory>();

            provider = services.BuildServiceProvider();
            scope = provider.CreateScope();

            var configuration = new ConfigurationBuilder().AddUserSecrets(typeof(CardDavClientTest).Assembly).Build();

            userName = configuration.GetSection("Cred:UserName").Get<string>();
            password = configuration.GetSection("Cred:Password").Get<string>();
            token = configuration.GetSection("Google:Token").Get<string>();
            gmail = configuration.GetSection("Google:Login").Get<string>();

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

            response = await client.PropfindAsync($"/addressbook/{userName}/", string.Empty, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
            Assert.Equal(2, response.AddressBooks.Count);

            response = await client.PropfindAsync(response.AddressBooks[1].Endpoint, string.Empty, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
            Assert.Equal(1, response.AddressBooks.Count);
            Assert.Equal(5, response.ResourceEndpoints.Count);

            response = await client.GetAsync(response.ResourceEndpoints[0].Endpoint, CancellationToken.None);
            Assert.True(response.IsSuccess);
            Assert.Single(response.VCardResponse);
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

            var responseVCard = response.VCardResponse[0];
            Assert.Equal(vCard.Name.FamilyNames, responseVCard.VCard.Name.FamilyNames);
            Assert.Equal(vCard.Name.GivenNames, responseVCard.VCard.Name.GivenNames);
            Assert.Equal(vCard.Name.AdditionalNames, responseVCard.VCard.Name.AdditionalNames);
            Assert.Equal(vCard.Name.HonorificPrefixes, responseVCard.VCard.Name.HonorificPrefixes);
            Assert.Equal(vCard.Name.HonorificSuffixes, responseVCard.VCard.Name.HonorificSuffixes);
            Assert.Equal(vCard.FullName, responseVCard.VCard.FullName);
            Assert.Equal(vCard.Phones, responseVCard.VCard.Phones, new PhonesEqualityComparer());
            Assert.Equal(vCard.Emails, responseVCard.VCard.Emails, new EmailsEqualityComparer());

            //у яндекса багнутый апдейт
            //var updateVCard = VCardBuilder.Create("BEGIN:VCARD\r\nVERSION:3.0\r\nUID:2312133421324668575897435\r\nN:Doe;John;;;\r\nFN:John Doe\r\nEMAIL:test@test.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nEND:VCARD\r\n").Build();
            //response = await client.UpdateContactAsync($"/addressbook/{userName}/addressbook/new", updateVCard, new CarddavRequest { ETag = response.eTag }, CancellationToken.None);
            //Assert.True(response.IsSuccess);

            //response = await client.GetAsync($"/addressbook/{userName}/addressbook/new", CancellationToken.None);
            //Assert.True(response.IsSuccess);
            //Assert.Equal("test@test.org", response.vCards.First().Emails.First().Email);

            response = await client.DeleteContactAsync($"/addressbook/{userName}/addressbook/new", CancellationToken.None);
            Assert.True(response.IsSuccess);

            response = await client.PropfindAsync($"/addressbook/{userName}/addressbook", string.Empty, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
            Assert.Equal(5, response.ResourceEndpoints.Count);
        }

        //For google test work you need to get somewhere valid access token
        [Fact]
        public async Task Success_Google_Basic()
        {
            var client = cardDavClientFactory.CreateClientWithAccessToken("https://www.googleapis.com/", token);

            var response = await client.PropfindAsync(".well-known/carddav", string.Empty, Depth.One, CancellationToken.None);

            var content = XmlQueryHelper.Propfind("prop", "current-user-principal", "getctag");


            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, Depth.One, CancellationToken.None);
            Assert.True(response.IsSuccess);

            var report = XmlQueryHelper.AddressCollection();

            response = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", report, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task Success_Google_CRUD()
        {
            var client = cardDavClientFactory.CreateClientWithAccessToken("https://www.googleapis.com/", token);

            var response = await client.PropfindAsync(".well-known/carddav", string.Empty, Depth.One, CancellationToken.None);

            var content = XmlQueryHelper.Propfind("prop", "current-user-principal", "principal-URL", "getctag");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var testPerson = "BEGIN:VCARD\r\nVERSION:3.0\r\nN:Doe;John;;;\r\nFN:John Doe\r\nORG:Example.com Inc.;\r\nTITLE:Imaginary test person\r\nEMAIL;type=INTERNET;type=WORK;type=pref:johnDoe@example.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nTEL;type=WORK:+1 (617) 555-1234\r\nTEL;type=CELL:+1 781 555 1212\r\nTEL;type=HOME:+1 202 555 1212\r\nEND:VCARD\r\n";

            var vCard = VCardBuilder.Create(testPerson).AddUId("2312133421324668575897435").Build();

            response = await client.AddContactAsync($"carddav/v1/principals/{gmail}/lists/default/new", vCard, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var eTagRequest = XmlQueryHelper.Propfind("prop", "getetag", "getcontenttype", "resourcetype");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default/new", eTagRequest, Depth.One, CancellationToken.None);

            var updateVCard = VCardBuilder.Create("BEGIN:VCARD\r\nVERSION:3.0\r\nUID:2312133421324668575897435\r\nN:Doe;John;;;\r\nFN:John Doe\r\nEMAIL:test@test.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nEND:VCARD\r\n").Build();
            var endpoint = response.ResourceEndpoints.First().Endpoint;
            var etag = response.ResourceEndpoints.First().Etag;
            response = await client.UpdateContactAsync(endpoint, updateVCard, etag, CancellationToken.None);
            Assert.True(response.IsSuccess);

            response = await client.GetAsync($"carddav/v1/principals/{gmail}/lists/default/new", CancellationToken.None);
            Assert.True(response.IsSuccess);
            Assert.Equal("test@test.org", response.VCardResponse.First().VCard.Emails.First().Email);

            response = await client.DeleteContactAsync(endpoint, CancellationToken.None);
            Assert.True(response.IsSuccess);

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default/", eTagRequest, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
            Assert.Equal(4, response.ResourceEndpoints.Count);

        }
        #region Helpers

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

        #endregion

        #region I think this is will useful later
        //propfind
        //var content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
        //"<d:propfind xmlns:d=\"DAV:\" xmlns:cs=\"http://calendarserver.org/ns/\">\r\n  " +
        //"<d:prop>\r\n    " +
        //" <d:current-user-principal />\r\n" +
        //" </d:prop>\r\n" +
        //"</ d:propfind>";

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

        //"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
        //" <A:propfind xmlns:A=\"DAV:\">\r\n " +
        //   " <A:prop>\r\n  " +
        //   " <A:current-user-principal/>\r\n " +
        //   " <A:principal-URL/>\r\n   " +
        //   " <A:resourcetype></A:resourcetype>\r\n " +
        //   " <A:getctag />\r\n " +
        //   " </A:prop>\r\n" +
        //" </A:propfind>\r\n";

        //"<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
        //"  <C:addressbook-query xmlns:D=\"DAV:\"\n          " +
        //"           xmlns:C=\"urn:ietf:params:xml:ns:carddav\">\n   " +
        //"  <D:prop>\n  " +
        //"     <D:getetag/>\n     " +
        //"   <C:address-data>\n      " +
        //"     </C:address-data>\n " +
        //"    </D:prop>\n    " +
        //"<C:filter>\r\n    <C:prop-filter name=\"FN\">\r\n    </C:prop-filter>    \r\n</C:filter>" +
        //"</C:addressbook-query>";

        //"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
        //    " <A:propfind xmlns:A=\"DAV:\">\r\n " +
        //    " <A:prop>\r\n" +
        //    " <A:getetag />\r\n " +
        //    " <A:getcontenttype />\r\n " +
        //    " <A:resourcetype />\r\n " +
        //    " </A:prop>\r\n" +
        //     " </A:propfind>\r\n";

        //"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
        //    " <A:propfind xmlns:A=\"DAV:\">\r\n " +
        //       " <A:prop>\r\n  " +
        //       " <A:current-user-principal/>\r\n " +
        //       " <A:principal-URL/>\r\n   " +
        //       " <A:resourcetype></A:resourcetype>\r\n " +
        //       " <A:getctag />\r\n " +
        //       " </A:prop>\r\n" +
        //    " </A:propfind>\r\n";

        #endregion
    }
}