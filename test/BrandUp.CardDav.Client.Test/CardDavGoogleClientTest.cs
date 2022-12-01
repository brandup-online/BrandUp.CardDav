using BrandUp.CardDav.Client.Extensions;
using BrandUp.CardDav.Client.Helpers;
using BrandUp.CardDav.Client.Models.Requests;
using BrandUp.VCard.Builders;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Client.Test
{
    public class CardDavGoogleClientTest : TestBase
    {
        private string token;
        private string gmail;

        public CardDavGoogleClientTest(ITestOutputHelper output) : base(output)
        {
            token = configuration.GetSection("Google:Token").Get<string>();
            gmail = configuration.GetSection("Google:Login").Get<string>();
        }

        //For google test work you need to get somewhere valid access token
        [Fact]
        public async Task Success_Google_Basic()
        {
            var client = cardDavClientFactory.CreateClientWithAccessToken("https://www.googleapis.com/", token);

            var response = await client.PropfindAsync(".well-known/carddav", string.Empty, Depth.One, CancellationToken.None);

            var content = XmlQueryHelper.Propfind("current-user-principal", "getetag", "getctag");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var report = XmlQueryHelper.AddressCollection(true);

            response = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", report, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task Success_Google_CRUD()
        {
            var client = cardDavClientFactory.CreateClientWithAccessToken("https://www.googleapis.com/", token);

            var response = await client.PropfindAsync(".well-known/carddav", string.Empty, Depth.One, CancellationToken.None);

            var content = XmlQueryHelper.Propfind("current-user-principal", "principal-URL", "getctag");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var vCard = VCardBuilder.Create(testPerson).SetUId("2312133421324668575897435").Build();

            response = await client.AddContactAsync($"carddav/v1/principals/{gmail}/lists/default/new", vCard, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var eTagRequest = XmlQueryHelper.Propfind("prop", "getetag", "getcontenttype", "resourcetype");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default/new", eTagRequest, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

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

        [Fact]
        public async Task Success_Google_Sync()
        {
            var client = cardDavClientFactory.CreateClientWithAccessToken("https://www.googleapis.com/", token);

            _ = await client.PropfindAsync(".well-known/carddav", string.Empty, Depth.One, CancellationToken.None);

            var content = XmlQueryHelper.Propfind("prop", "current-user-principal", "principal-URL", "getctag");

            var response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var propfind = XmlQueryHelper.Propfind("sync-token");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", propfind, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
            var syncRequest0 = XmlQueryHelper.SyncCollection();

            response = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", syncRequest0, Depth.One, CancellationToken.None);

            var syncRequest = XmlQueryHelper.SyncCollection(response.SyncToken, "infinite");

            response = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", syncRequest, Depth.One, CancellationToken.None);

            //Assert.True(response.IsSuccess);
        }
    }
}
