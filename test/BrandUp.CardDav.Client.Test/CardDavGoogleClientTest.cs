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
        private string pass;

        readonly CardDavClient client;

        public CardDavGoogleClientTest(ITestOutputHelper output) : base(output)
        {
            token = configuration.GetSection("Google:Token").Get<string>();// ?? throw new ArgumentNullException(nameof(token));
            gmail = configuration.GetSection("Google:Login").Get<string>() ?? throw new ArgumentNullException(nameof(gmail));
            pass = configuration.GetSection("Google:Password").Get<string>() ?? throw new ArgumentNullException(nameof(pass));

            client = cardDavClientFactory.CreateClientWithCredentials("https://www.googleapis.com/", gmail, pass);
        }

        //For google test work you need to get somewhere valid access token
        [Fact]
        public async Task Success_Google_Basic()
        {
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

            #region Init

            var response = await client.PropfindAsync(".well-known/carddav", string.Empty, Depth.One, CancellationToken.None);

            var content = XmlQueryHelper.Propfind("current-user-principal", "principal-URL", "getctag");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            #endregion

            #region Create

            var vCard = VCardBuilder.Create(testPerson).SetUId("2312133421324668575897435").Build();
            var name = RandomName;
            response = await client.AddContactAsync($"carddav/v1/principals/{gmail}/lists/default/{name}", vCard, CancellationToken.None);

            Assert.True(response.IsSuccess);

            #endregion

            #region Read

            var eTagRequest = XmlQueryHelper.Propfind("prop", "getetag", "getcontenttype", "resourcetype");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default/{name}", eTagRequest, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            #endregion

            #region Update

            var updateVCard = VCardBuilder.Create("BEGIN:VCARD\r\nVERSION:3.0\r\nUID:2312133421324668575897435\r\nN:Doe;John;;;\r\nFN:John Doe\r\nEMAIL:test@test.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nEND:VCARD\r\n").Build();
            var endpoint = response.Content.ResourceEndpoints.First().Endpoint;
            var etag = response.Content.ResourceEndpoints.First().Etag;
            response = await client.UpdateContactAsync(endpoint, updateVCard, etag, CancellationToken.None);
            Assert.True(response.IsSuccess);

            var vCardResponse = await client.GetAsync($"carddav/v1/principals/{gmail}/lists/default/{name}", CancellationToken.None);
            Assert.NotNull(vCardResponse);
            Assert.Equal("test@test.org", vCardResponse.Emails.First().Email);

            #endregion

            #region Delete

            response = await client.DeleteContactAsync(endpoint, CancellationToken.None);
            Assert.True(response.IsSuccess);

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default/", eTagRequest, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
            Assert.Equal(4, response.Content.ResourceEndpoints.Count);

            #endregion
        }

        [Fact]
        public async Task Success_Google_Sync()
        {
            _ = await client.PropfindAsync(".well-known/carddav", string.Empty, Depth.One, CancellationToken.None);

            var content = XmlQueryHelper.Propfind("prop", "current-user-principal", "principal-URL", "getctag");

            var response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var propfind = XmlQueryHelper.Propfind("sync-token");

            response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", propfind, Depth.One, CancellationToken.None);

            Assert.True(response.IsSuccess);
            //var syncRequest0 = XmlQueryHelper.SyncCollection();

            //response = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", syncRequest0, Depth.One, CancellationToken.None);
        }

        private static Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string RandomName => new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());

    }
}
