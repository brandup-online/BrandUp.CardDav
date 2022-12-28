using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.VCard;
using BrandUp.CardDav.VCard.Builders;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Client.Test
{
    public class CardDavGoogleClientTest : TestBase
    {
        //private string token;
        private string gmail;
        private string password;

        readonly CardDavClient client;

        public CardDavGoogleClientTest(ITestOutputHelper output) : base(output)
        {
            //token = configuration.GetSection("Google:Token").Get<string>() ?? throw new ArgumentNullException(nameof(token));
            gmail = configuration.GetSection("Google:Login").Get<string>() ?? throw new ArgumentNullException(nameof(gmail));
            password = configuration.GetSection("Google:Password").Get<string>() ?? throw new ArgumentNullException(nameof(password));

            client = cardDavClientFactory.CreateClientWithCredentials("https://www.googleapis.com/", gmail, password);
        }

        [Fact]
        public async Task Success_Google_Basic()
        {
            _ = await client.PropfindAsync(".well-known/carddav", PropfindRequest.Create(Depth.One), CancellationToken.None);

            var content = PropfindRequest.Create(Depth.One, Prop.ETag, Prop.CTag, Prop.CurrentUserPrincipal);

            var propfindResponse = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode.ToString());
            Assert.True(propfindResponse.IsSuccess);

            #region Empty filter

            var filter = new FilterBody();
            filter.AddPropFilter(VCardProperty.FN, FilterMatchType.All, TextMatch.Create("", TextMatchType.Contains));
            var report = ReportRequest.CreateQuery(PropList.Create(Prop.CTag, Prop.ETag), new AddressData(), filter);

            var reportResponse = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", report, CancellationToken.None);

            output.WriteLine(reportResponse.StatusCode.ToString());
            Assert.True(reportResponse.IsSuccess);
            Assert.NotEmpty(reportResponse.Body.Resources);

            #endregion
        }

        [Fact]
        public async Task Success_Google_NotEmptyFilter()
        {
            var filter = new FilterBody();
            filter.AddPropFilter(VCardProperty.EMAIL, FilterMatchType.All, TextMatch.Create("me", TextMatchType.Contains));
            var report = ReportRequest.CreateQuery(PropList.Create(Prop.CTag, Prop.ETag, new AddressData()),
                                    new AddressData(),
                                    filter);

            var reportResponse = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", report, CancellationToken.None);

            output.WriteLine(reportResponse.StatusCode.ToString());
            Assert.True(reportResponse.IsSuccess);
            Assert.Single(reportResponse.Body.Resources);
            Assert.NotNull(reportResponse.Body.Resources.First().CardModel);
        }

        [Fact]
        public async Task Success_Google_Limit()
        {
            var filter = new FilterBody();
            filter.AddPropFilter(VCardProperty.FN, FilterMatchType.All, TextMatch.Create("", TextMatchType.Contains));
            var report = ReportRequest.CreateQuery(
                                    PropList.Create(Prop.CTag, Prop.ETag),
                                    new AddressData(),
                                    filter,
                                    2);

            var reportResponse = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", report, CancellationToken.None);

            output.WriteLine(reportResponse.StatusCode.ToString());
            Assert.True(reportResponse.IsSuccess);
            Assert.Equal(3, reportResponse.Body.Resources.Count);
            Assert.NotNull(reportResponse.Body.Resources.First().CardModel);
        }

        [Fact]
        public async Task Success_Google_CRUD()
        {
            #region Init

            var propfindResponse = await client.PropfindAsync(".well-known/carddav", PropfindRequest.Create(Depth.One), CancellationToken.None);

            var content = PropfindRequest.Create(Depth.One, Prop.CTag, Prop.CurrentUserPrincipal, Prop.PrincipalUrl);

            propfindResponse = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode.ToString());
            Assert.True(propfindResponse.IsSuccess);

            #endregion

            #region Create

            var vCard = VCardBuilder.Create(testPerson).SetUId("2312133421324668575897435").Build();
            var name = RandomName;
            var createResponse = await client.AddContactAsync($"carddav/v1/principals/{gmail}/lists/default/{name}", vCard, CancellationToken.None);

            output.WriteLine(createResponse.StatusCode.ToString());
            Assert.True(createResponse.IsSuccess);

            #endregion

            #region Read

            var eTagRequest = PropfindRequest.Create(Depth.One, Prop.ETag, Prop.ResourceType);

            propfindResponse = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default/{name}", eTagRequest, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode.ToString());
            Assert.True(propfindResponse.IsSuccess);

            #endregion

            #region Update

            var updateVCard = VCardBuilder.Create("BEGIN:VCARD\r\nVERSION:3.0\r\nUID:2312133421324668575897435\r\nN:Doe;John;;;\r\nFN:John Doe\r\nEMAIL:test@test.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nEND:VCARD\r\n").Build();
            var endpoint = propfindResponse.Body.Resources.First().Endpoint;
            var etag = propfindResponse.Body.Resources.First().FoundProperties[Prop.ETag];
            var updateResponse = await client.UpdateContactAsync(endpoint, updateVCard, etag, CancellationToken.None);

            output.WriteLine(updateResponse.StatusCode.ToString());
            Assert.True(updateResponse.IsSuccess);

            var vCardResponse = await client.GetAsync($"carddav/v1/principals/{gmail}/lists/default/{name}", CancellationToken.None);
            Assert.NotNull(vCardResponse);
            Assert.Equal("test@test.org", vCardResponse.VCard.Emails.First().Email);

            #endregion

            #region Delete

            var deleteResponse = await client.DeleteContactAsync(endpoint, CancellationToken.None);

            output.WriteLine(deleteResponse.StatusCode.ToString());
            Assert.True(deleteResponse.IsSuccess);

            propfindResponse = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default/", eTagRequest, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode.ToString());
            Assert.True(propfindResponse.IsSuccess);
            Assert.Equal(5, propfindResponse.Body.Resources.Count());

            #endregion
        }

        [Fact]
        public async Task Success_Google_Sync()
        {
            _ = await client.PropfindAsync(".well-known/carddav", PropfindRequest.Create(Depth.One), CancellationToken.None);

            var content = PropfindRequest.Create(Depth.One, Prop.CTag, Prop.CurrentUserPrincipal, Prop.PrincipalUrl);

            var response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", content, CancellationToken.None);

            output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);

            //Это возможно не так работает 
            //var propfind = XmlQueryHelper.Propfind("sync-token");

            //response = await client.PropfindAsync($"carddav/v1/principals/{gmail}/lists/default", propfind, Depth.One, CancellationToken.None);

            //output.WriteLine(response.StatusCode);
            //Assert.True(response.IsSuccess);
            //var syncRequest0 = XmlQueryHelper.SyncCollection();

            //response = await client.ReportAsync($"carddav/v1/principals/{gmail}/lists/default", syncRequest0, Depth.One, CancellationToken.None);
        }

        private static Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string RandomName => new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());

    }
}
