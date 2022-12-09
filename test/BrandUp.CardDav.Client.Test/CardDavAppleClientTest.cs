using BrandUp.CardDav.Client.Extensions;
using BrandUp.CardDav.Client.Helpers;
using BrandUp.CardDav.Transport.Models.Body;
using BrandUp.CardDav.Transport.Models.Headers;
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

            output.WriteLine(options.StatusCode);
            Assert.True(options.IsSuccess);
            Assert.NotEmpty(options.AllowHeaderValue);
            output.WriteLine(string.Join(" ", options.AllowHeaderValue));
            Assert.NotEmpty(options.DavHeaderValue);
            output.WriteLine(string.Join(" ", options.DavHeaderValue));

            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var response = await client.PropfindAsync($"{login}/carddavhome/", request, CancellationToken.None);

            output.WriteLine(response.StatusCode);
            Assert.True(response.IsSuccess);

            request = PropfindRequest.Create(Depth.One, Prop.ETag);

            response = await client.PropfindAsync(response.Resources[0].Endpoint, request, CancellationToken.None);

            Assert.True(response.IsSuccess);

            var report = XmlQueryHelper.AddressCollection(true);

            var reportResponse = await client.ReportAsync(response.Resources[1].Endpoint, report, Depth.One.Value, CancellationToken.None);

            output.WriteLine(reportResponse.StatusCode);
            Assert.True(reportResponse.IsSuccess);
        }

        [Fact]
        public async Task Success_Allprop()
        {
            var options = await client.OptionsAsync(CancellationToken.None);
            Assert.True(options.IsSuccess);

            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var response = await client.PropfindAsync($"{login}/carddavhome/", request, CancellationToken.None);

            output.WriteLine(response.StatusCode);
            Assert.True(response.IsSuccess);

            var allpropRequest = PropfindRequest.AllProp(Depth.One);

            var allpropResponse = await client.PropfindAsync(response.Resources[0].Endpoint, allpropRequest, CancellationToken.None);

            output.WriteLine(allpropResponse.StatusCode);
            Assert.True(allpropResponse.IsSuccess);
        }


        [Fact]
        public async Task Success_CRUD()
        {
            #region Init

            var options = await client.OptionsAsync(CancellationToken.None);

            output.WriteLine(options.StatusCode);
            Assert.True(options.IsSuccess);
            Assert.NotEmpty(options.AllowHeaderValue);
            Assert.NotEmpty(options.DavHeaderValue);

            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfindResponse = await client.PropfindAsync($"{login}/carddavhome/", request, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode);
            Assert.True(propfindResponse.IsSuccess);


            request = PropfindRequest.Create(Depth.One, Prop.ETag);

            propfindResponse = await client.PropfindAsync(propfindResponse.Resources[0].Endpoint, request, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode);
            Assert.True(propfindResponse.IsSuccess);

            #endregion

            #region Create 

            var newUserEndpoint = propfindResponse.Resources[1].Endpoint + "new.vcf";

            var vCard = VCardBuilder.Create(testPerson).AddEmail("milo@milo.com", Kind.Home).SetUId("2312133421324668575897435").Build();

            var response1 = await client.AddContactAsync(newUserEndpoint, vCard, CancellationToken.None);

            Assert.True(response1.IsSuccess);

            #endregion

            #region Read

            var eTagRequest = PropfindRequest.Create(Depth.One, Prop.ETag, Prop.ResourceType);
            propfindResponse = await client.PropfindAsync(newUserEndpoint, eTagRequest, CancellationToken.None);

            output.WriteLine(propfindResponse.StatusCode);
            Assert.True(propfindResponse.IsSuccess);
            Assert.Single(propfindResponse.Resources);

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

            var etag = propfindResponse.Resources.First().FoundProperties[Prop.ETag];
            var updateResponse = await client.UpdateContactAsync(newUserEndpoint, updateVCard, etag, CancellationToken.None); //просто заменяет контакт

            output.WriteLine(updateResponse.StatusCode);
            Assert.True(updateResponse.IsSuccess);

            vCardResponse = await client.GetAsync(newUserEndpoint, CancellationToken.None);
            Assert.NotNull(vCardResponse);
            Assert.Equal("test@test.org", vCardResponse.Emails.First().Email);

            #endregion

            #region Delete

            var deleteResponse = await client.DeleteContactAsync(newUserEndpoint, CancellationToken.None);

            output.WriteLine(deleteResponse.StatusCode);
            Assert.True(deleteResponse.IsSuccess);

            #endregion
        }
    }
}
