using BrandUp.CardDav.Server.Controllers.Tests;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using BrandUp.CardDav.Transport.Models.Requests;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Tests
{
    public class CommonTests : ControllerTestBase
    {
        public CommonTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Success_TakeAllContactsFromCollections()
        {
            var request = PropfindRequest.AllProp(Depth.One);

            var response = await Client.PropfindAsync("Principal/User/Collections", request, CancellationToken.None);

            Output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);

            string addressbookEndpoint = "";
            foreach (var resourse in response.Body.Resources)
            {
                if (resourse.Endpoint.EndsWith("Default"))
                    addressbookEndpoint = resourse.Endpoint;
            }

            //передалать создание Report запроса
            var reportRequest = ReportRequest.CreateQuery(PropList.Create(Prop.ETag), new AddressData(), new FilterBody());
            var reportResponse = await Client.ReportAsync(addressbookEndpoint, reportRequest, CancellationToken.None);

            Output.WriteLine(reportResponse.StatusCode.ToString());
            Assert.True(reportResponse.IsSuccess);

            var vCards = reportResponse.Body.Resources.Select(r => r.CardModel).Where(c => c != null);
            Assert.Equal(3, vCards.Count());

            Assert.All(vCards, Assert.NotNull);
            Assert.Collection(vCards,
                vCard => Assert.Equal(vCard, TestVCards.VCard1),
                vCard => Assert.Equal(vCard, TestVCards.VCard2),
                vCard => Assert.Equal(vCard, TestVCards.VCard3));
        }

        [Fact]
        public async Task Success_MakeCollectionAndAddContacts()
        {
            var endpoint = "Principal/User/Collections/New";

            var mkcolResponse = await Client.MkcolAsync(endpoint);

            Output.WriteLine(mkcolResponse.StatusCode.ToString());
            Assert.True(mkcolResponse.IsSuccess);

            var addContactResult = await Client.AddContactAsync(string.Join("/", endpoint, "card1"), TestVCards.VCard1, CancellationToken.None);

            Output.WriteLine(addContactResult.StatusCode.ToString());
            Assert.True(addContactResult.IsSuccess);

            addContactResult = await Client.AddContactAsync(string.Join("/", endpoint, "card2"), TestVCards.VCard2, CancellationToken.None);

            Output.WriteLine(addContactResult.StatusCode.ToString());
            Assert.True(addContactResult.IsSuccess);

            addContactResult = await Client.AddContactAsync(string.Join("/", endpoint, "card3"), TestVCards.VCard3, CancellationToken.None);

            Output.WriteLine(addContactResult.StatusCode.ToString());
            Assert.True(addContactResult.IsSuccess);

            var reportRequest = ReportRequest.CreateQuery(PropList.Create(Prop.ETag), new AddressData(), new FilterBody());
            var reportResponse = await Client.ReportAsync(endpoint, reportRequest, CancellationToken.None);

            Output.WriteLine(reportResponse.StatusCode.ToString());
            Assert.True(reportResponse.IsSuccess);

            var vCards = reportResponse.Body.Resources.Select(r => r.CardModel).Where(c => c != null);
            Assert.Equal(3, vCards.Count());

            Assert.All(vCards, Assert.NotNull);
            Assert.Collection(vCards,
                vCard => Assert.Equal(vCard, TestVCards.VCard1),
                vCard => Assert.Equal(vCard, TestVCards.VCard2),
                vCard => Assert.Equal(vCard, TestVCards.VCard3));
        }
    }
}
