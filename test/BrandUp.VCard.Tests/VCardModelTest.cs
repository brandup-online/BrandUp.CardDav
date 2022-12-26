namespace BrandUp.CardDav.VCard.Tests
{
    public class VCardModelTest : IAsyncLifetime
    {
        const string vCard = "BEGIN:VCARD\r\n" +
                             "VERSION:3.0\r\n" +
                             "N:Doe;John;;;\r\n" +
                             "FN:John Doe\r\n" +
                             "ORG:Example.com Inc.;\r\n" +
                             "TITLE:Imaginary test person\r\n" +
                             "EMAIL;type=WORK;type=INTERNET;type=pref:johnDoe@example.org\r\n" +
                             "TEL;type=WORK;type=pref:+1 617 555 1212\r\n" +
                             "TEL;type=WORK:+1 (617) 555-1234\r\n" +
                             "TEL;type=CELL:+1 781 555 1212\r\n" +
                             "TEL;type=HOME:+1 202 555 1212\r\n" +
                             "END:VCARD\r\n";


        [Fact]
        public async Task Success()
        {
            var result = await VCardParser.ParseAsync(vCard, CancellationToken.None);

            Assert.NotNull(result);

            var serialized = result.ToString();

            Assert.NotNull(serialized);
            Assert.NotEmpty(serialized);
            Assert.Equal(vCard, serialized, true);
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
    }
}
