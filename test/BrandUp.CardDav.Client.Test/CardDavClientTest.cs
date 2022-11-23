using BrandUp.CardDav.Client.Extensions;
using BrandUp.CardDav.Client.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Client.Test
{
    public class CardDavClientTest : IAsyncLifetime
    {
        readonly ITestOutputHelper output;

        private ICardDavClientFactory cardDavClientFactory;

        private ServiceProvider provider;
        private IServiceScope scope;

        private string userName;
        private string password;

        public CardDavClientTest(ITestOutputHelper output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));

            var services = new ServiceCollection();

            services.AddHttpClient();
            services.AddLogging();

            services.AddScoped<ICardDavClientFactory, CardDavClientFactory>();

            provider = services.BuildServiceProvider();
            scope = provider.CreateScope();

            var configuration = new ConfigurationBuilder().AddUserSecrets(typeof(CardDavClientTest).Assembly).Build();

            userName = configuration.GetSection("Cred:UserName").Get<string>();
            password = configuration.GetSection("Cred:Password").Get<string>();

            cardDavClientFactory = scope.ServiceProvider.GetRequiredService<ICardDavClientFactory>();
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

        [Fact]
        public async void Success_Yandex()
        {
            var client = cardDavClientFactory.CreateClientWithCredentials("https://carddav.yandex.ru/", userName, password);

            var response = await client.OptionsAsync(CancellationToken.None);

            output.WriteLine(response);

            response = await client.PropfindAsync("/addressbook/dmitryschashev@yandex.ru/addressbook/", new Models.CarddavRequest { Depth = "0" }, CancellationToken.None);
            output.WriteLine(response);

            response = await client.PropfindAsync("/addressbook/dmitryschashev@yandex.ru/addressbook/", new Models.CarddavRequest { Depth = "1" }, CancellationToken.None);
            output.WriteLine(response);

            response = await client.GetAsync("/addressbook/dmitryschashev@yandex.ru/addressbook/YA-1", CancellationToken.None);
            output.WriteLine(response);
        }

        #region I think this is be useful later
        //propfind
        //var content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
        //"<d:propfind xmlns:d=\"DAV:\" xmlns:cs=\"http://calendarserver.org/ns/\">\r\n  " +
        //"<d:prop>\r\n    " +
        //" <d:displayname />\r\n    " +
        //" <cs:getctag />\r\n " +
        //" <d:sync-token />\r\n" +
        //" </d:prop>\r\n</d:propfind>";

        //report
        //        var content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
        //"  <C:addressbook-query xmlns:D=\"DAV:\"\n          " +
        //"           xmlns:C=\"urn:ietf:params:xml:ns:carddav\">\n   " +
        //"  <D:prop>\n  " +
        //"     <D:getetag/>\n     " +
        //"  <C:address-data>\n      " +
        //"     </C:address-data>\n " +
        //"    </D:prop>\n    " +
        //"<C:filter>\r\n    <C:prop-filter name=\"FN\">\r\n    </C:prop-filter>    \r\n</C:filter>" +
        //"</C:addressbook-query>";
        #endregion
    }
}