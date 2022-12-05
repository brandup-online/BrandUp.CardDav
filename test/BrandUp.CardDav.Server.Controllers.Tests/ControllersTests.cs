using BrandUp.CardDav.Client;
using BrandUp.CardDav.Client.Helpers;
using BrandUp.CardDav.Client.Models.Requests;
using BrandUp.CardDav.Client.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Controllers
{
    public class ControllersTests : IAsyncLifetime
    {
        readonly ClientApplicationFactory factory;
        readonly CardDavClient cardDavClient;
        readonly ITestOutputHelper output;

        public ControllersTests(ITestOutputHelper output)
        {
            factory = new();
            this.output = output;

            var client = factory.CreateClient();
            cardDavClient = new CardDavClient(client, NullLogger<CardDavClient>.Instance, new CardDavCredentialsOptions
            {
                BaseUrl = client.BaseAddress.ToString(),
                Login = "",
                Password = ""
            });
        }

        #region IAsyncLifetime

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        [Fact]
        public async Task Success_Options()
        {
            var options = await cardDavClient.OptionsAsync(CancellationToken.None);

            output.WriteLine(options.StatusCode);
            Assert.True(options.IsSuccess);
            Assert.NotEmpty(options.AllowHeaderValue);
            output.WriteLine(string.Join(" ", options.AllowHeaderValue));
            Assert.NotEmpty(options.DavHeaderValue);
            output.WriteLine(string.Join(" ", options.DavHeaderValue));
        }

        [Fact]
        public async Task Success_Propfind()
        {
            var request = XmlQueryHelper.Propfind("getetag");

            var propfind = await cardDavClient.PropfindAsync("Principal/user", request, Depth.Zero, CancellationToken.None);

            output.WriteLine(propfind.StatusCode);
            Assert.True(propfind.IsSuccess);
        }
    }
}