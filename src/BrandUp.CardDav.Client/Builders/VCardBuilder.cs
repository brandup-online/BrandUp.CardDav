using BrandUp.Carddav.Client.Models;
using BrandUp.Carddav.Client.Parsers;

namespace BrandUp.Carddav.Client.Builders
{
    public class VCardBuilder : IVCardBuilder
    {
        private string rawVCard;

        private const string begin = "BEGIN:VCARD\r\n";
        private const string end = "END:VCARD\r\n";
        private string version = "VERSION:4.0\r\n";
        private string emails = string.Empty;
        private string phones = string.Empty;
        private string uId = string.Empty;
        private string fullName = string.Empty;
        private string name = string.Empty;


        internal VCardBuilder()
        {
            rawVCard = string.Empty;
        }

        internal VCardBuilder(string rawVCard) : this()
        {
            var array = rawVCard.Split("\r\n");

            version = array.Where(s => s.StartsWith("VERSION")).Single() + "\r\n";
            fullName = array.Where(s => s.StartsWith("FN:")).Single() + "\r\n";
            name = array.Where(s => s.StartsWith("N:")).Single() + "\r\n";
            phones = string.Join("\r\n", array.Where(s => s.StartsWith("TEL"))) + "\r\n";
            emails = string.Join("\r\n", array.Where(s => s.StartsWith("EMAIL"))) + "\r\n";
        }

        #region IVCardBuilder members

        public static IVCardBuilder Create()
        {
            return new VCardBuilder();
        }

        public static IVCardBuilder Create(string rawVCard)
        {
            return new VCardBuilder(rawVCard);
        }

        public static IVCardBuilder CreateFromFile(string filepath)
        {
            using var file = File.OpenRead(filepath);

            return CreateFromStream(file);
        }

        public static IVCardBuilder CreateFromStream(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var raw = reader.ReadToEnd();

            return new VCardBuilder(raw);
        }

        public IVCardBuilder AddUId(string uId)
        {
            if (this.uId != string.Empty)
                throw new ArgumentException("Cannot add two FN properties to one vcard");
            this.uId = $"UID:{uId}\r\n";

            return this;
        }

        public IVCardBuilder AddFullName(string Name)
        {
            if (fullName != string.Empty)
                throw new ArgumentException("Cannot add two FN properties to one vcard");
            fullName = $"FN:{Name}\r\n";

            return this;
        }

        public IVCardBuilder AddName(string familyNames, string givenNames, string additionalNames, string honorificPrefixes, string honorificSuffixes)
        {
            if (fullName != string.Empty)
                throw new ArgumentException("Cannot add two N properties to one vcard");
            fullName = "N:" + string.Join(';', familyNames, givenNames, additionalNames, honorificPrefixes, honorificSuffixes) + "\r\n";

            return this;
        }

        public IVCardBuilder AddEmail(string email, Kind type)
        {
            emails += $"EMAIL;type={type.ToString().ToUpper()}:{email}\r\n";

            return this;
        }

        public IVCardBuilder AddPhone(string phone, Kind type)
        {
            emails += $"TEL;type={type.ToString().ToUpper()}:{phone}\r\n";

            return this;
        }

        public VCard Build()
        {
            rawVCard = begin + version + uId + name + fullName + phones + emails + end;

            return VCardParser.Parse(rawVCard);
        }

        #endregion
    }

    public interface IVCardBuilder
    {
        static abstract IVCardBuilder Create();
        static abstract IVCardBuilder Create(string rawVCard);
        static abstract IVCardBuilder CreateFromFile(string filepath);
        static abstract IVCardBuilder CreateFromStream(Stream stream);
        IVCardBuilder AddUId(string uId);
        IVCardBuilder AddFullName(string Name);
        IVCardBuilder AddName(string familyNames, string givenNames, string additionalNames, string honorificPrefixes, string honorificSuffixes);
        IVCardBuilder AddPhone(string phone, Kind type);
        IVCardBuilder AddEmail(string email, Kind type);
        VCard Build();

    }
}
