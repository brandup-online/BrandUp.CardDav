using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
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
            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfind = await Client.PropfindAsync("Principal/User/Collections", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }

        [Fact]
        public async Task Success_Report()
        {
            var filter = new Filter()
            {

            };
            var request = ReportRequest.CreateQuery(Depth.Zero, PropList.Create(Prop.ETag, Prop.CTag), filter);

            var propfind = await Client.ReportAsync("Principal/User/Collections", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }

    }
}
