namespace BrandUp.VCard.Builders
{
    public class VCardBuilder : IVCardBuilder
    {
        private VCard vCard;

        internal VCardBuilder()
        {
            vCard = new();
        }

        internal VCardBuilder(string rawVCard) : this()
        {
            this.vCard = VCardParser.Parse(rawVCard);
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

            vCard.Name.FamilyNames = familyNames.Split(',').ToList();
            vCard.Name.GivenNames = givenNames.Split(',').ToList();
            vCard.Name.AdditionalNames = additionalNames.Split(',').ToList();
            vCard.Name.HonorificPrefixes = honorificPrefixes.Split(',').ToList();
            vCard.Name.HonorificSuffixes = honorificSuffixes.Split(',').ToList();

            vCard.FormattedName = string.Join(" ", vCard.Name.HonorificPrefixes, vCard.Name.GivenNames, vCard.Name.AdditionalNames, vCard.Name.FamilyNames, vCard.Name.HonorificSuffixes);

            return this;
        }

        public IVCardBuilder SetUId(string uId)
        {
            if (uId == null) throw new ArgumentNullException(nameof(uId));

            vCard.UId = uId;

            return this;
        }

        public IVCardBuilder AddEmail(string email, Kind type, params string[] types)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            vCard.Emails.Add(new() { Email = email, Kind = type, Types = types });

            return this;
        }

        public IVCardBuilder UpdateEmail(string oldEmail, string email, Kind type, params string[] types)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (oldEmail == null) throw new ArgumentNullException(nameof(oldEmail));

            vCard.Emails.ToList().RemoveAll(e => e.Email == oldEmail);

            vCard.Emails.Add(new() { Email = email, Kind = type, Types = types });

            return this;
        }

        public IVCardBuilder AddPhone(string phone, Kind type, params string[] types)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));

            vCard.Phones.Add(new() { Phone = phone, Kind = type, Types = types });

            return this;
        }

        public IVCardBuilder UpdatePhone(string oldPhone, string phone, Kind type, params string[] types)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));
            if (oldPhone == null) throw new ArgumentNullException(nameof(oldPhone));

            vCard.Phones.ToList().RemoveAll(e => e.Phone == oldPhone);

            vCard.Phones.Add(new() { Phone = phone, Kind = type, });

            return this;
        }


        public VCard Build() => vCard;

        #endregion
    }

    public interface IVCardBuilder
    {
        static abstract IVCardBuilder Create();
        static abstract IVCardBuilder Create(string rawVCard);
        static abstract IVCardBuilder CreateFromFile(string filepath);
        static abstract IVCardBuilder CreateFromStream(Stream stream);
        IVCardBuilder SetUId(string uId);
        IVCardBuilder SetName(string name);
        IVCardBuilder SetName(string familyNames, string givenNames, string additionalNames = "", string honorificPrefixes = "", string honorificSuffixes = "");
        IVCardBuilder AddPhone(string phone, Kind type, params string[] types);
        IVCardBuilder UpdatePhone(string oldPhone, string phone, Kind type, params string[] types);
        IVCardBuilder AddEmail(string email, Kind type, params string[] types);
        IVCardBuilder UpdateEmail(string oldEmail, string email, Kind type, params string[] types);
        VCard Build();

    }
}
