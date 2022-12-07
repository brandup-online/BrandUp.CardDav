using BrandUp.CardDav.Transport.Models;
using BrandUp.CardDav.Transport.Models.Body;
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
            var request = PropfindRequest.Create(Depth.Zero, Prop.ETag);

            var propfind = await Client.PropfindAsync("Principal/user", request, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }
    }
}
