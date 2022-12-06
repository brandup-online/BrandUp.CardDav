using BrandUp.CardDav.Client.Helpers;
using BrandUp.CardDav.Client.Models.Requests;
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
            var request = XmlQueryHelper.Propfind("getetag");

            var propfind = await Client.PropfindAsync("Principal/user", request, Depth.Zero, CancellationToken.None);

            Output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }
    }
}
