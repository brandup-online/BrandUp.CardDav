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
                             "END:VCARD";


        [Fact]
        public void Success()
        {
            var result = new VCardModel(vCard);

            Assert.NotNull(result);

            var serialized = result.ToString();

            Assert.NotNull(serialized);
            Assert.NotEmpty(serialized);

            Assert.Collection(vCard.Split("\r\n"),
               c => Assert.Equal("BEGIN:VCARD", c),
               c => Assert.Equal("VERSION:3.0", c),
               c => Assert.Equal("N:Doe;John;;;", c),
               c => Assert.Equal("FN:John Doe", c),
               c => Assert.Equal("ORG:Example.com Inc.;", c),
               c => Assert.Equal("TITLE:Imaginary test person", c),
               c => Assert.Equal("EMAIL;type=WORK;type=INTERNET;type=pref:johnDoe@example.org", c),
               c => Assert.Equal("TEL;type=WORK;type=pref:+1 617 555 1212", c),
               c => Assert.Equal("TEL;type=WORK:+1 (617) 555-1234", c),
               c => Assert.Equal("TEL;type=CELL:+1 781 555 1212", c),
               c => Assert.Equal("TEL;type=HOME:+1 202 555 1212", c),
               c => Assert.Equal("END:VCARD", c));
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
