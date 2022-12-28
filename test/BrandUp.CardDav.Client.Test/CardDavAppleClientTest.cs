using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.VCard;
using BrandUp.CardDav.VCard.Builders;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using static BrandUp.CardDav.Client.Test.CardDavYandexClientTest;

namespace BrandUp.CardDav.Client.Test
{
    public class CardDavAppleClientTest : TestBase
    {
        private string login;
        private string password;

        readonly CardDavClient client;

        public CardDavAppleClientTest(ITestOutputHelper output) : base(output)
        {
            login = configuration.GetSection("Apple:Login").Get<string>() ?? throw new ArgumentNullException(nameof(login));
            password = configuration.GetSection("Apple:Password").Get<string>() ?? throw new ArgumentNullException(nameof(password));

            client = cardDavClientFactory.CreateClientWithCredentials("https://contacts.icloud.com/", login, password);
        }

        [Fact]
        public async Task Success_Basic()
        {
            var options = await client.OptionsAsync(CancellationToken.None);

            output.WriteLine(options.StatusCode.ToString());
            Assert.True(options.IsSuccess);
            Assert.NotEmpty(options.AllowHeaderValue);
            output.WriteLine(string.Join(" ", options.AllowHeaderValue));
            Assert.NotEmpty(options.DavHeaderValue);
            output.WriteLine(string.Join(" ", options.DavHeaderValue));

            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var response = await client.PropfindAsync($"{login}/carddavhome/", request, CancellationToken.None);

            output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);

            request = PropfindRequest.Create(Depth.One, Prop.ETag);

            response = await client.PropfindAsync(response.Body.Resources[0].Endpoint, request, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var filter = new FilterBody();

            filter.AddPropFilter(VCardProperty.FN, FilterMatchType.All, TextMatch.Create("", TextMatchType.Contains));
            var report = ReportRequest.CreateQuery(PropList.Create(Prop.CTag, Prop.ETag), new AddressData(), filter);

            var reportResponse = await client.ReportAsync(response.Body.Resources[1].Endpoint, report, CancellationToken.None);

            output.WriteLine(reportResponse.StatusCode.ToString());
            Assert.True(reportResponse.IsSuccess);

            filter = new FilterBody();

            filter.AddPropFilter(VCardProperty.FN, FilterMatchType.All, TextMatch.Create("ma", TextMatchType.Contains));
            report = ReportRequest.CreateQuery(PropList.Create(Prop.CTag, Prop.ETag), new AddressData(), filter);

            reportResponse = await client.ReportAsync(response.Body.Resources[1].Endpoint, report, CancellationToken.None);

            output.WriteLine(reportResponse.StatusCode.ToString());
            Assert.True(reportResponse.IsSuccess);
            Assert.Single(reportResponse.Body.Resources);
            Assert.NotNull(reportResponse.Body.Resources.First().CardModel);

            request = PropfindRequest.AllProp(Depth.One);

            response = await client.PropfindAsync(response.Body.Resources[1].Endpoint, request, CancellationToken.None);

            Assert.True(response.IsSuccess);

            report = ReportRequest.CreateMultiget(PropList.Create(Prop.CTag, Prop.ETag), new AddressData(), response.Body.Resources[1].Endpoint);

            reportResponse = await client.ReportAsync(response.Body.Resources[1].Endpoint, report, CancellationToken.None);

            output.WriteLine(reportResponse.StatusCode.ToString());
            Assert.True(reportResponse.IsSuccess);
            Assert.Single(reportResponse.Body.Resources);
            Assert.NotNull(reportResponse.Body.Resources.First().CardModel);
        }

        //[Fact]
        //public async Task Success_MakeCollection()
        //{
        //    var options = await client.OptionsAsync(CancellationToken.None);
        //    Assert.True(options.IsSuccess);

        //    var request = PropfindRequest.Create(Depth.One, Prop.ETag);

        //    var response = await client.PropfindAsync($"{login}/carddavhome/", request, CancellationToken.None);
        //    Assert.True(response.IsSuccess);

        //    var mkcolRequest = new MkcolRequest
        //    {
        //        Headers = new Dictionary<string, string>(),
        //        Body = new SetPropBody("myAddresBook", "desc")
        //    };
        //    var mkcol = await client.MkcolAsync(response.Body.Resources[0].Endpoint, mkcolRequest, CancellationToken.None);

        //    Assert.True(mkcol.IsSuccess);
        //}

        [Fact]
        public async Task Success_Allprop()
        {
            var options = await client.OptionsAsync(CancellationToken.None);
            Assert.True(options.IsSuccess);

            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var response = await client.PropfindAsync($"{login}/carddavhome/", request, CancellationToken.None);

            output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);

            var allpropRequest = PropfindRequest.AllProp(Depth.One);

            var allpropResponse = await client.PropfindAsync(response.Body.Resources[0].Endpoint, allpropRequest, CancellationToken.None);

            output.WriteLine(allpropResponse.StatusCode.ToString());
            Assert.True(allpropResponse.IsSuccess);
        }

        [Fact]
        public async Task Success_CRUD()
        {
            #region Init

            var options = await client.OptionsAsync(CancellationToken.None);

            output.WriteLine(options.StatusCode.ToString());
            Assert.True(options.IsSuccess);
            Assert.NotEmpty(options.AllowHeaderValue);
            Assert.NotEmpty(options.DavHeaderValue);

            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfindResponse = await client.PropfindAsync($"{login}/carddavhome/", request, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode.ToString());
            Assert.True(propfindResponse.IsSuccess);


            request = PropfindRequest.Create(Depth.One, Prop.ETag);

            propfindResponse = await client.PropfindAsync(propfindResponse.Body.Resources[0].Endpoint, request, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode.ToString());
            Assert.True(propfindResponse.IsSuccess);

            #endregion

            #region Create 

            var newUserEndpoint = propfindResponse.Body.Resources[1].Endpoint + "new.vcf";

            var vCard = VCardBuilder.Create(testPerson).AddEmail("milo@milo.com", Kind.Home).SetUId("2312133421324668575897435").Build();

            var response1 = await client.AddContactAsync(newUserEndpoint, vCard, CancellationToken.None);

            Assert.True(response1.IsSuccess);

            #endregion

            #region Read

            var eTagRequest = PropfindRequest.Create(Depth.One, Prop.ETag, Prop.ResourceType);
            propfindResponse = await client.PropfindAsync(newUserEndpoint, eTagRequest, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode.ToString());
            Assert.True(propfindResponse.IsSuccess);
            Assert.Single(propfindResponse.Body.Resources);

            var vCardResponse = await client.GetAsync(newUserEndpoint, CancellationToken.None);
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

            var updateVCard = VCardBuilder.Create("BEGIN:VCARD\r\nVERSION:3.0\r\nUID:2312133421324668575897435\r\nN:Doe;John;;;\r\nFN:John Doe\r\nEMAIL;type=INTERNET;type=WORK;type=pref:test@test.org\r\nTEL;type=WORK;type=pref:+1 617 555 1212\r\nEND:VCARD\r\n").Build();

            var etag = propfindResponse.Body.Resources.First().FoundProperties[Prop.ETag];
            var updateResponse = await client.UpdateContactAsync(newUserEndpoint, updateVCard, etag, CancellationToken.None); //просто заменяет контакт

            output.WriteLine(updateResponse.StatusCode.ToString());
            Assert.True(updateResponse.IsSuccess);

            vCardResponse = await client.GetAsync(newUserEndpoint, CancellationToken.None);
            Assert.NotNull(vCardResponse);
            Assert.Equal("test@test.org", vCardResponse.VCard.Emails.First().Email);

            #endregion

            #region Delete

            var deleteResponse = await client.DeleteContactAsync(newUserEndpoint, CancellationToken.None);

            output.WriteLine(deleteResponse.StatusCode.ToString());
            Assert.True(deleteResponse.IsSuccess);

            #endregion
        }
    }
}
