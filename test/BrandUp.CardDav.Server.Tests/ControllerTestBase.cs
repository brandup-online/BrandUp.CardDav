using BrandUp.CardDav.Client;
using BrandUp.CardDav.Client.Options;
using BrandUp.Extensions.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Controllers.Tests
{
    public abstract class ControllerTestBase : IAsyncLifetime
    {
        protected ClientApplicationFactory factory;

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
                Login = "User",
                Password = "Password"
            });
        }

        #region IAsyncLifetime members

        public async Task InitializeAsync()
        {
            var scopeFactory = factory.Server.Services.GetService<IServiceScopeFactory>();
            using var migrateScope = scopeFactory.CreateScope();

            var migrationExecutor = migrateScope.ServiceProvider.GetRequiredService<MigrationExecutor>();

            await migrationExecutor.UpAsync();
        }

        public async Task DisposeAsync()
        {
            await factory.DisposeAsync();
        }

        #endregion
    }
}
