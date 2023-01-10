using System.Diagnostics.CodeAnalysis;

namespace BrandUp.CardDav.VCard
{
    public class ParseTest : IAsyncLifetime
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
        public void Success_String()
        {
            var result = new VCardModel(vCard);

            Check(result);
        }

        [Fact]
        public async Task Success_Stream()
        {
            #region Preparation

            using var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(vCard);
            writer.Flush();
            stream.Position = 0;

            #endregion

            var result = new VCardModel(stream);

            Check(result);
        }

        private void Check(VCardModel result)
        {
            Assert.NotNull(result);
            Assert.Equal(VCardVersion.VCard3, result.Version);

            Assert.Single(result.Name.FamilyNames);
            Assert.Equal("Doe", result.Name.FamilyNames.First());
            Assert.Single(result.Name.GivenNames);
            Assert.Equal("John", result.Name.GivenNames.First());

            Assert.Equal("John Doe", result.FormattedName);

            Assert.Collection(result.Phones,
                p => Assert.Equal(p, new VCardPhone() { Phone = "+1 617 555 1212", Kind = Kind.Work, Types = new[] { TelType.Pref } }, new PhoneComparer()),
                p => Assert.Equal(p, new VCardPhone() { Phone = "+1 (617) 555-1234", Kind = Kind.Work, Types = new TelType[0] }, new PhoneComparer()),
                p => Assert.Equal(p, new VCardPhone() { Phone = "+1 781 555 1212", Kind = null, Types = new[] { TelType.Cell } }, new PhoneComparer()),
                p => Assert.Equal(p, new VCardPhone() { Phone = "+1 202 555 1212", Kind = Kind.Home, Types = new TelType[0] }, new PhoneComparer()));

            Assert.Single(result.Emails);
            Assert.Collection(result.Emails, p => Assert.Equal(p, new VCardEmail() { Email = "johnDoe@example.org", Kind = Kind.Work }, new EmailComparer()));
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

        private class PhoneComparer : IEqualityComparer<VCardPhone>
        {
            public bool Equals(VCardPhone x, VCardPhone y)
            {
                if (x.Types != null && y.Types != null)
                {
                    if (x.Types.Length != y.Types.Length)
                        return false;
                    for (var i = 0; i < x.Types.Length; i++)
                    {
                        if (!y.Types.Contains(x.Types[i]))
                            return false;
                    }
                }
                else if (x.Types != null && y.Types == null || x.Types == null && y.Types != null)
                    return false;

                return x.Phone == y.Phone && x.Kind == y.Kind;
            }

            public int GetHashCode([DisallowNull] VCardPhone obj)
            {
                return obj.GetHashCode();
            }
        }

        private class EmailComparer : IEqualityComparer<VCardEmail>
        {
            public bool Equals(VCardEmail x, VCardEmail y)
            {
                return x.Email == y.Email && x.Kind == y.Kind;
            }

            public int GetHashCode([DisallowNull] VCardEmail obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}