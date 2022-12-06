namespace BrandUp.CardDav.VCard.Builders
{
    public class VCardBuilder : IVCardBuilder
    {
        private VCardModel vCard;

        internal VCardBuilder(Version version = Version.VCard3)
        {
            vCard = new() { Version = version };
        }

        internal VCardBuilder(string rawVCard) : this()
        {
            this.vCard = VCardParser.Parse(rawVCard);
        }

        internal VCardBuilder(VCardModel vCard)
        {
            this.vCard = vCard ?? throw new ArgumentNullException(nameof(vCard));
        }

        #region Static members

        public static IVCardBuilder Create(Version version = Version.VCard3)
           => new VCardBuilder(version);

        public static IVCardBuilder Create(string rawVCard)
        => new VCardBuilder(rawVCard);

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

        public static IVCardBuilder Update(VCardModel vCard) => new VCardBuilder(vCard);

        #endregion

        #region IVCardBuilder members

        public IVCardBuilder SetName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            vCard.FormattedName = name;

            return this;
        }

        public IVCardBuilder SetName(string familyNames, string givenNames, string additionalNames = "", string honorificPrefixes = "", string honorificSuffixes = "")
        {
            if (familyNames == null) throw new ArgumentNullException(nameof(familyNames));
            if (givenNames == null) throw new ArgumentNullException(nameof(givenNames));
            if (additionalNames == null) throw new ArgumentNullException(nameof(additionalNames));
            if (honorificPrefixes == null) throw new ArgumentNullException(nameof(honorificPrefixes));
            if (honorificSuffixes == null) throw new ArgumentNullException(nameof(honorificSuffixes));

            var name = new VCardName();

            name.FamilyNames = familyNames.Split(',').ToList();
            name.GivenNames = givenNames.Split(',').ToList();
            name.AdditionalNames = additionalNames.Split(',').ToList();
            name.HonorificPrefixes = honorificPrefixes.Split(',').ToList();
            name.HonorificSuffixes = honorificSuffixes.Split(',').ToList();

            vCard.Name = name;

            List<string> names = new();
            names.AddRange(vCard.Name.HonorificPrefixes);
            names.AddRange(vCard.Name.GivenNames);
            names.AddRange(vCard.Name.AdditionalNames);
            names.AddRange(vCard.Name.FamilyNames);
            names.AddRange(vCard.Name.HonorificSuffixes);

            names.RemoveAll(x => x == "");

            vCard.FormattedName = string.Join(" ", names);

            return this;
        }

        public IVCardBuilder SetUId(string uId)
        {
            if (uId == null) throw new ArgumentNullException(nameof(uId));

            vCard.UId = uId;

            return this;
        }

        public IVCardBuilder AddEmail(string email, Kind? type, params string[] types)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            vCard.Emails.Add(new() { Email = email, Kind = type, Types = types });

            return this;
        }

        public IVCardBuilder UpdateEmail(string oldEmail, string email, Kind? type, params string[] types)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (oldEmail == null) throw new ArgumentNullException(nameof(oldEmail));

            vCard.Emails.ToList().RemoveAll(e => e.Email == oldEmail);

            vCard.Emails.Add(new() { Email = email, Kind = type, Types = types });

            return this;
        }

        public IVCardBuilder AddPhone(string phone, Kind? type, params string[] types)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));

            vCard.Phones.Add(new() { Phone = phone, Kind = type, Types = types });

            return this;
        }

        public IVCardBuilder UpdatePhone(string oldPhone, string phone, Kind? type, params string[] types)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));
            if (oldPhone == null) throw new ArgumentNullException(nameof(oldPhone));

            vCard.Phones.ToList().RemoveAll(e => e.Phone == oldPhone);

            vCard.Phones.Add(new() { Phone = phone, Kind = type, });

            return this;
        }


        public VCardModel Build() => vCard;

        #endregion
    }

    public interface IVCardBuilder
    {
        IVCardBuilder SetUId(string uId);
        IVCardBuilder SetName(string name);
        IVCardBuilder SetName(string familyNames, string givenNames, string additionalNames = "", string honorificPrefixes = "", string honorificSuffixes = "");
        IVCardBuilder AddPhone(string phone, Kind? type, params string[] types);
        IVCardBuilder UpdatePhone(string oldPhone, string phone, Kind? type, params string[] types);
        IVCardBuilder AddEmail(string email, Kind? type, params string[] types);
        IVCardBuilder UpdateEmail(string oldEmail, string email, Kind? type, params string[] types);
        VCardModel Build();

    }
}
