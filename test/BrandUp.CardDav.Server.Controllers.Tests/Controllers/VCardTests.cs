using BrandUp.CardDav.VCard.Builders;
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
            var vCard = VCardBuilder.Create().SetName("Doe", "Jonh").AddPhone("79921213321", VCard.Kind.Work).Build();

            var response = await Client.AddContactAsync("Principal/user/Collections/default/test", vCard, CancellationToken.None);

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
            var vCard = VCardBuilder.Create().SetName("Doe", "Jonh").AddPhone("79921213321", VCard.Kind.Work).Build();

            var response = await Client.UpdateContactAsync("Principal/user/Collections/default/test", vCard, "\"etag\"", CancellationToken.None);

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
