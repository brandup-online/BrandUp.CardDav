using BrandUp.CardDav.Server.Controllers.Tests;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Tests.Controllers
{
    public class HomeTests : ControllerTestBase
    {
        public HomeTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async Task Success_Options()
        {
            var options = await Client.OptionsAsync(CancellationToken.None);

            Output.WriteLine(options.StatusCode.ToString());
            Assert.True(options.IsSuccess);
            Assert.NotEmpty(options.AllowHeaderValue);
            Output.WriteLine(string.Join(" ", options.AllowHeaderValue));
            Assert.NotEmpty(options.DavHeaderValue);
            Output.WriteLine(string.Join(" ", options.DavHeaderValue));
        }
    }
}