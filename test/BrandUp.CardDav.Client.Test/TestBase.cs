using BrandUp.CardDav.Client.Extensions;
using BrandUp.CardDav.Client.Factory;
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

        protected const string testPerson = "BEGIN:VCARD\r\n" +
            "VERSION:3.0\r\n" +
            "N:Doe;John;;;\r\n" +
            "FN:John Doe\r\n" +
            "ORG:Example.com Inc.;\r\n" +
            "TITLE:Imaginary test person\r\n" +
            "EMAIL;type=INTERNET;type=WORK;type=pref:johnDoe@example.org\r\n" +
            "TEL;type=WORK;type=pref:+1 617 555 1212\r\n" +
            "TEL;type=WORK:+1 (617) 555-1234\r\n" +
            "TEL;type=CELL:+1 781 555 1212\r\n" +
            "TEL;type=HOME:+1 202 555 1212\r\n" +
            "END:VCARD\r\n";

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
