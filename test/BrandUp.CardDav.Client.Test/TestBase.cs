using BrandUp.Carddav.Client.Extensions;
using BrandUp.Carddav.Client.Factory;
using BrandUp.Carddav.Client.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Client.Test
{
    public abstract class TestBase : IAsyncLifetime
    {
        readonly ServiceProvider provider;
        readonly IServiceScope scope;

        protected ICardDavClientFactory cardDavClientFactory;
        protected ITestOutputHelper output;
        protected IConfiguration configuration;

        public TestBase(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));

            var services = new ServiceCollection();

            services.AddLogging();

            services.AddCardDavClient();

            provider = services.BuildServiceProvider();
            scope = provider.CreateScope();

            cardDavClientFactory = scope.ServiceProvider.GetRequiredService<ICardDavClientFactory>();

            configuration = new ConfigurationBuilder().AddUserSecrets(typeof(CardDavYandexClientTest).Assembly).Build();
        }

        #region IAsyncLifetime members

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await provider.DisposeAsync();
            scope.Dispose();
        }

        #endregion
    }
}
