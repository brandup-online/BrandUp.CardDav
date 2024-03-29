using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.VCard;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Client.Test
{
    public class CardDavYandexClientTest : TestBase
    {
        private string userName;
        private string password;

        public CardDavYandexClientTest(ITestOutputHelper output) : base(output)
        {
            userName = configuration.GetSection("Yandex:UserName").Get<string>() ?? throw new ArgumentNullException(nameof(userName));
            password = configuration.GetSection("Yandex:Password").Get<string>() ?? throw new ArgumentNullException(nameof(password));
        }

        [Fact]
        public async Task Success_Yandex_Basic()
        {
            var client = cardDavClientFactory.CreateClientWithCredentials("https://carddav.yandex.ru/", userName, password);

            var options = await client.OptionsAsync(CancellationToken.None);

            output.WriteLine(options.StatusCode.ToString());
            Assert.True(options.IsSuccess);
            Assert.NotEmpty(options.AllowHeaderValue);
            Assert.NotEmpty(options.DavHeaderValue);

            var response = await client.PropfindAsync($"/addressbook/{userName}/", PropfindRequest.AllProp(Depth.One), CancellationToken.None);

            output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);
            Assert.Equal(2, response.Body.Resources.Count());
            var addressBookEndpoint = response.Body.Resources.Where(r => r.FoundProperties[Prop.ResourceType].Contains("addressbook")).FirstOrDefault().Endpoint;

            response = await client.PropfindAsync(addressBookEndpoint, PropfindRequest.Create(Depth.One, Prop.ETag, Prop.DisplayName), CancellationToken.None);

            output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);
            Assert.Equal(6, response.Body.Resources.Count());

            var vCardResponse = await client.GetAsync(response.Body.Resources[1].Endpoint, CancellationToken.None);
            Assert.NotNull(vCardResponse);
        }

        [Fact]
        public async Task Success_Yandex_CRUD()
        {
            var client = cardDavClientFactory.CreateClientWithCredentials("https://carddav.yandex.ru/", userName, password);

            #region Init

            var options = await client.OptionsAsync(CancellationToken.None);

            output.WriteLine(options.StatusCode.ToString());
            Assert.True(options.IsSuccess);
            Assert.NotEmpty(options.AllowHeaderValue);
            Assert.NotEmpty(options.DavHeaderValue);

            #endregion

            #region Create

            var vCard = new VCardModel(testPerson);
            vCard.AddPropperty(CardProperty.UID, "2312133421324668575897435");

            var createResponse = await client.AddContactAsync($"/addressbook/{userName}/addressbook/new", vCard, CancellationToken.None);

            output.WriteLine(createResponse.StatusCode.ToString());
            Assert.True(createResponse.IsSuccess);

            #endregion

            #region Read 

            var vCardResponse = await client.GetAsync($"/addressbook/{userName}/addressbook/new", CancellationToken.None);
            Assert.NotNull(vCardResponse);

            Assert.Equal(vCard.Name.FamilyNames, vCardResponse.VCard.Name.FamilyNames);
            Assert.Equal(vCard.Name.GivenNames, vCardResponse.VCard.Name.GivenNames);
            Assert.Equal(vCard.Name.AdditionalNames, vCardResponse.VCard.Name.AdditionalNames);
            Assert.Equal(vCard.Name.HonorificPrefixes, vCardResponse.VCard.Name.HonorificPrefixes);
            Assert.Equal(vCard.Name.HonorificSuffixes, vCardResponse.VCard.Name.HonorificSuffixes);
            Assert.Equal(vCard.FormattedName, vCardResponse.VCard.FormattedName);
            Assert.Equal(vCard.Phones, vCardResponse.VCard.Phones, new PhonesEqualityComparer());
            Assert.Equal(vCard.Emails, vCardResponse.VCard.Emails, new EmailsEqualityComparer());

            #endregion

            #region Update

            //� ������� �������� ������
            //var updateVCard = VCardBuilder.Create("BEGIN:VCARD\r\nVERSION:3.0\r\nUID:2312133421324668575897435\r\nN:Doe;John;;;\r\nFN:John Doe\r\nEMAIL:test@test.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nEND:VCARD\r\n").Build();
            //response = await client.UpdateContactAsync($"/addressbook/{userName}/addressbook/new", updateVCard, new CarddavRequest { ETag = response.eTag }, CancellationToken.None);
            //Assert.True(response.IsSuccess);

            //response = await client.GetAsync($"/addressbook/{userName}/addressbook/new", CancellationToken.None);
            //Assert.True(response.IsSuccess);
            //Assert.Equal("test@test.org", response.vCards.First().Emails.First().Email);

            #endregion

            #region Delete

            var deleteResponse = await client.DeleteContactAsync($"/addressbook/{userName}/addressbook/new", CancellationToken.None);

            output.WriteLine(deleteResponse.StatusCode.ToString());
            Assert.True(deleteResponse.IsSuccess);

            var propfindResponse = await client.PropfindAsync($"/addressbook/{userName}/addressbook", PropfindRequest.Create(Depth.One), CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode.ToString());
            Assert.True(propfindResponse.IsSuccess);
            Assert.Equal(6, propfindResponse.Body.Resources.Count());

            #endregion
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