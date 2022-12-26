using BrandUp.CardDav.Server.Controllers.Tests;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.VCard.Builders;
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

            var vCard = VCardBuilder.Create().SetName("Doe", "Jonh").AddPhone("79921213321", VCard.Kind.Work).Build();

            var response = await Client.AddContactAsync("Principal/User/Collections/Default/test", vCard, CancellationToken.None);

            Output.WriteLine(response.StatusCode);
            Assert.True(response.IsSuccess);

            #endregion

            #region Read

            var vCardResponse = await Client.GetAsync("Principal/User/Collections/Default/test", CancellationToken.None);

            Output.WriteLine(response.StatusCode);
            Assert.NotNull(vCardResponse);

            var propfind = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfinResponse = await Client.PropfindAsync("Principal/User/Collections/Default/test", propfind);

            Output.WriteLine(response.StatusCode);
            Assert.True(propfinResponse.IsSuccess);

            #endregion

            #region Update

            vCard = VCardBuilder.Create().SetName("Deo", "Jinh").AddPhone("79921213251", VCard.Kind.Work).Build();

            var eTag = propfinResponse.Body.Resources.First().FoundProperties[Prop.ETag];
            response = await Client.UpdateContactAsync("Principal/User/Collections/Default/test", vCard, eTag, CancellationToken.None);

            Output.WriteLine(response.StatusCode);
            Assert.True(response.IsSuccess);

            #endregion

            #region Delete

            response = await Client.DeleteContactAsync("Principal/User/Collections/Default/test", CancellationToken.None);

            Output.WriteLine(response.StatusCode);
            Assert.True(response.IsSuccess);

            #endregion
        }
    }
}
