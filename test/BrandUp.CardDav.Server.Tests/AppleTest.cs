using BrandUp.CardDav.Server.Controllers.Tests;
using Xunit.Abstractions;

namespace BrandUp.CardDav.Server.Tests
{
    public class AppleTest : ControllerTestBase
    {
        private const string request = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
            "<A:propfind xmlns:A=\"DAV:\">\r\n" +
            "<A:prop>\r\n" +
            "<A:current-user-principal/>\r\n" +
            "<A:principal-URL/>\r\n" +
            "<A:resourcetype/>\r\n" +
            "</A:prop>\r\n" +
            "</A:propfind>";

        public AppleTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task WellKnownRequest()
        {
            //Для разнообразия решил не использовать наш клиент.

            using var response = await ExecuteAsync("/.well-known/carddav");

            Output.WriteLine(await response.Content.ReadAsStringAsync());
            Output.WriteLine(response.StatusCode.ToString());

            Assert.False(response.IsSuccessStatusCode);
        }


        [Fact]
        public async Task PrincipalsSuccess()
        {
            using var response = await ExecuteAsync("/principals");

            Output.WriteLine(await response.Content.ReadAsStringAsync());
            Output.WriteLine(response.StatusCode.ToString());

            Assert.False(response.IsSuccessStatusCode);
        }

        #region Helpers

        async Task<HttpResponseMessage> ExecuteAsync(string endpoint)
        {
            using var httpclient = factory.CreateClient();

            using var message = new HttpRequestMessage(new HttpMethod("PROPFIND"), endpoint)
            {
                Content = new StringContent(request),
            };

            message.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
            message.Headers.Authorization = new($"Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "User", "Password"))));

            return await httpclient.SendAsync(message);
        }

        #endregion
    }
}
