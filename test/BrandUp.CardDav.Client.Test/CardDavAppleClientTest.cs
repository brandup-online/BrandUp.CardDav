using BrandUp.CardDav.Client.Extensions;
using BrandUp.CardDav.Client.Helpers;
using BrandUp.CardDav.Client.Models.Requests;
using BrandUp.VCard;
using BrandUp.VCard.Builders;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using static BrandUp.CardDav.Client.Test.CardDavYandexClientTest;

namespace BrandUp.CardDav.Client.Test
{
    public class CardDavAppleClientTest : TestBase
    {
        private string login;
        private string password;

        public CardDavAppleClientTest(ITestOutputHelper output) : base(output)
        {
            login = configuration.GetSection("Apple:Login").Get<string>();
            password = configuration.GetSection("Apple:Password").Get<string>();
        }

        [Fact]
        public async Task Success_Basic()
        {
            var client = cardDavClientFactory.CreateClientWithCredentials("https://contacts.icloud.com/", login, password);

            var response = await client.OptionsAsync(CancellationToken.None);

            Assert.True(response.IsSuccess);

            var request = XmlQueryHelper.Propfind("getetag");

            response = await client.PropfindAsync($"{login}/carddavhome/", request, Depth.Zero, CancellationToken.None);

            Assert.True(response.IsSuccess);

            response = await client.PropfindAsync(response.Content.ResourceEndpoints[0].Endpoint, request, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var report = XmlQueryHelper.AddressCollection(true);

            response = await client.ReportAsync(response.Content.ResourceEndpoints[1].Endpoint, report, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task Success_CRUD()
        {
            var client = cardDavClientFactory.CreateClientWithCredentials("https://contacts.icloud.com/", login, password);

            #region Init

            var response = await client.OptionsAsync(CancellationToken.None);

            Assert.True(response.IsSuccess);

            var request = XmlQueryHelper.Propfind("getetag");

            response = await client.PropfindAsync($"{login}/carddavhome/", request, Depth.Zero, CancellationToken.None);

            Assert.True(response.IsSuccess);

            response = await client.PropfindAsync(response.Content.ResourceEndpoints[0].Endpoint, request, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            #endregion

            #region Create 

            var newUserEndpoint = response.Content.ResourceEndpoints[1].Endpoint + "new.vcf";

            var vCard = VCardBuilder.Create(testPerson).AddEmail("milo@milo.com", Kind.Home).SetUId("2312133421324668575897435").Build();

            response = await client.AddContactAsync(newUserEndpoint, vCard, CancellationToken.None);

            Assert.True(response.IsSuccess);

            #endregion

            #region Read

            var eTagRequest = XmlQueryHelper.Propfind("getetag", "getcontenttype", "resourcetype");
            response = await client.PropfindAsync(newUserEndpoint, eTagRequest, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var vCardResponse = await client.GetAsync(newUserEndpoint, CancellationToken.None);
            Assert.NotNull(vCardResponse);

            Assert.Equal(vCard.Name.FamilyNames, vCardResponse.Name.FamilyNames);
            Assert.Equal(vCard.Name.GivenNames, vCardResponse.Name.GivenNames);
            Assert.Equal(vCard.Name.AdditionalNames, vCardResponse.Name.AdditionalNames);
            Assert.Equal(vCard.Name.HonorificPrefixes, vCardResponse.Name.HonorificPrefixes);
            Assert.Equal(vCard.Name.HonorificSuffixes, vCardResponse.Name.HonorificSuffixes);
            Assert.Equal(vCard.FormattedName, vCardResponse.FormattedName);
            Assert.Equal(vCard.Phones, vCardResponse.Phones, new PhonesEqualityComparer());
            Assert.Equal(vCard.Emails, vCardResponse.Emails, new EmailsEqualityComparer());

            #endregion

            #region Update 
            var updateVCard = VCardBuilder.Create("BEGIN:VCARD\r\nVERSION:3.0\r\nUID:2312133421324668575897435\r\nN:Doe;John;;;\r\nFN:John Doe\r\nEMAIL;type=INTERNET;type=WORK;type=pref:test@test.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nEND:VCARD\r\n").Build();
            var etag = response.ETag;
            response = await client.UpdateContactAsync(newUserEndpoint, updateVCard, etag, CancellationToken.None); //просто заменяет контакт
            Assert.True(response.IsSuccess);

            vCardResponse = await client.GetAsync(newUserEndpoint, CancellationToken.None);
            Assert.NotNull(vCardResponse);
            Assert.Equal("test@test.org", vCardResponse.Emails.First().Email);

            #endregion

            #region Delete

            response = await client.DeleteContactAsync(newUserEndpoint, CancellationToken.None);
            Assert.True(response.IsSuccess);

            #endregion
        }
    }
}
