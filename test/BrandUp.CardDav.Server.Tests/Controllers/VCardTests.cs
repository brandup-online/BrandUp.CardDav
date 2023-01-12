using BrandUp.CardDav.Server.Controllers.Tests;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.VCard;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Tests.Controllers
{
    public class VCardTests : ControllerTestBase
    {
        public VCardTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Success_CRUD()
        {
            #region Create 

            var vCard = new VCardModel();

            vCard.AddPropperty(CardProperty.FN, "Jonh Doe");
            vCard.AddPropperty(CardProperty.TEL, "79921213321", new VCardParameter(CardParameter.TYPE, "Work"));

            var response = await Client.AddContactAsync("principals/User/Collections/Default/test", vCard, CancellationToken.None);

            Output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);

            #endregion

            #region Read

            var vCardResponse = await Client.GetAsync("principals/User/Collections/Default/test", CancellationToken.None);

            Output.WriteLine(response.StatusCode.ToString());
            Assert.NotNull(vCardResponse);

            var propfind = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfinResponse = await Client.PropfindAsync("principals/User/Collections/Default/test", propfind);

            Output.WriteLine(response.StatusCode.ToString());
            Assert.True(propfinResponse.IsSuccess);

            #endregion

            #region Update

            vCard = new VCardModel();
            vCard.AddPropperty(CardProperty.N, "Jinh;Deo");
            vCard.AddPropperty(CardProperty.FN, "Deo Jinh");
            vCard.AddPropperty(CardProperty.TEL, "79921213251", new VCardParameter(CardParameter.TYPE, "Work"));

            var eTag = propfinResponse.Body.Resources.First().FoundProperties[Prop.ETag];
            response = await Client.UpdateContactAsync("principals/User/Collections/Default/test", vCard, eTag, CancellationToken.None);

            Output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);

            #endregion

            #region Delete

            response = await Client.DeleteContactAsync("principals/User/Collections/Default/test", CancellationToken.None);

            Output.WriteLine(response.StatusCode.ToString());
            Assert.True(response.IsSuccess);

            #endregion
        }
    }
}
