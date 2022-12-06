using BrandUp.CardDav.Client;
using BrandUp.CardDav.Client.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Controllers.Tests
{
    public abstract class ControllerTestBase : IAsyncLifetime
    {
        private ClientApplicationFactory factory;

        protected CardDavClient Client { get; set; }
        protected ITestOutputHelper Output { get; set; }

        public ControllerTestBase(ITestOutputHelper output)
        {
            factory = new();
            Output = output;

            var client = factory.CreateClient();
            Client = new CardDavClient(client, NullLogger<CardDavClient>.Instance, new CardDavCredentialsOptions
            {
                BaseUrl = client.BaseAddress.ToString(),
                Login = "",
                Password = ""
            });
        }


        #region IAsyncLifetime members

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await factory.DisposeAsync();
        }

        #endregion
    }
}
