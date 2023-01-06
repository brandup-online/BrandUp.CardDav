using BrandUp.CardDav.VCard.Builders;

namespace BrandUp.CardDav.VCard.Tests
{
    public class VCardBuilderTest : IAsyncLifetime
    {

        const string vCard = "BEGIN:VCARD\r\n" +
                             "VERSION:3.0\r\n" +
                             "N:Doe;John;;;\r\n" +
                             "FN:John Doe\r\n" +
                             "EMAIL;type=WORK;type=INTERNET;type=pref:johnDoe@example.org\r\n" +
                             "TEL;type=WORK;type=pref:+1 617 555 1212\r\n" +
                             "TEL;type=WORK:+1 (617) 555-1234\r\n" +
                             "TEL;type=CELL:+1 781 555 1212\r\n" +
                             "TEL;type=HOME:+1 202 555 1212\r\n" +
                             "END:VCARD";

        [Fact]
        public void Success()
        {
            var vCardBuilded = VCardBuilder.Create(VCardVersion.VCard3).SetName("Doe", "John")
                .AddEmail("johnDoe@example.org", Kind.Work, "INTERNET", "pref")
                .AddPhone("+1 617 555 1212", Kind.Work, TelType.Pref)
                .AddPhone("+1 (617) 555-1234", Kind.Work)
                .AddPhone("+1 781 555 1212", null, TelType.Cell)
                .AddPhone("+1 202 555 1212", Kind.Home)
                .Build();

            var serialized = vCardBuilded.ToString();

            Assert.Collection(vCard.Split("\r\n"),
               c => Assert.Equal("BEGIN:VCARD", c),
               c => Assert.Equal("VERSION:3.0", c),
               c => Assert.Equal("N:Doe;John;;;", c),
               c => Assert.Equal("FN:John Doe", c),
               c => Assert.Equal("EMAIL;type=WORK;type=INTERNET;type=pref:johnDoe@example.org", c),
               c => Assert.Equal("TEL;type=WORK;type=pref:+1 617 555 1212", c),
               c => Assert.Equal("TEL;type=WORK:+1 (617) 555-1234", c),
               c => Assert.Equal("TEL;type=CELL:+1 781 555 1212", c),
               c => Assert.Equal("TEL;type=HOME:+1 202 555 1212", c),
               c => Assert.Equal("END:VCARD", c));
        }

        #region IAsyncLifetime

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
