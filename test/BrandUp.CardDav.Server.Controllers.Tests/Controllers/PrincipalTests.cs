using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Requests;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Controllers.Tests.Controllers
{
    public class PrincipalTests : ControllerTestBase
    {
        public PrincipalTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async Task Success_Propfind()
        {
            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag, Prop.PrincipalUrl);

            var propfind = await Client.PropfindAsync("Principal/user", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }
    }
}
