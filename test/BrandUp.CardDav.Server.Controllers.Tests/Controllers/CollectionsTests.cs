using BrandUp.CardDav.Client.Helpers;
using BrandUp.CardDav.Transport.Models.Requests;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Controllers.Tests.Controllers
{
    public class CollectionsTests : ControllerTestBase
    {
        public CollectionsTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async Task Success_Propfind()
        {
            var request = XmlQueryHelper.Propfind("getetag");

            var propfind = await Client.PropfindAsync("Principal/user/Collections/", request, Depth.Zero, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }

        [Fact]
        public async Task Success_Report()
        {
            var request = XmlQueryHelper.AddressCollection();

            var propfind = await Client.ReportAsync("Principal/user/Collections/", request, Depth.Zero, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }

    }
}
