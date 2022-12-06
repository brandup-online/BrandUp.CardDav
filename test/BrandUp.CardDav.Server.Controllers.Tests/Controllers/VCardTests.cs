using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Controllers.Tests.Controllers
{
    public class VCardTests : ControllerTestBase
    {
        public VCardTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Success_Create()
        {
            var response = await Client.AddContactAsync("Principal/user/Collections/default/test", new VCard.VCardModel(), CancellationToken.None);

            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task Success_Get()
        {
            var vCard = await Client.GetAsync("Principal/user/Collections/default/test", CancellationToken.None);

            Assert.NotNull(vCard);
        }

        [Fact]
        public async Task Success_Update()
        {
            var response = await Client.UpdateContactAsync("Principal/user/Collections/default/test", new VCard.VCardModel(), "etag", CancellationToken.None);

            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task Success_Delete()
        {
            var response = await Client.DeleteContactAsync("Principal/user/Collections/default/test", CancellationToken.None);

            Assert.True(response.IsSuccess);
        }
    }
}
