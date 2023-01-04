namespace BrandUp.CardDav.VCard.Builders
{
    public class VCardBuilder : IVCardBuilder
    {
        private VCardModel vCard;
        Dictionary<CardProperty, IEnumerable<VCardLine>> vCard1;

        internal VCardBuilder(VCardVersion version = VCardVersion.VCard3)
        {
            vCard = new() { Version = version };
        }

        internal VCardBuilder(string rawVCard) : this()
        {
            this.vCard1 = VCardParser.Parse(rawVCard).VCardDictionary;
        }

        internal VCardBuilder(VCardModel vCard)
        {
            this.vCard = vCard ?? throw new ArgumentNullException(nameof(vCard));
            if (vCard == null)
                throw new ArgumentNullException(nameof(vCard));

            vCard1 = vCard.VCardDictionary;
        }

        #region Static members

        public static IVCardBuilder Create(VCardVersion version = VCardVersion.VCard3)
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

            AddLine(CardProperty.UID, uId, true);

            return this;
        }

        public IVCardBuilder AddEmail(string email, params string[] types)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            AddLine(CardProperty.EMAIL, email, types, false);
            return this;
        }

        public IVCardBuilder UpdateEmail(string oldEmail, string email, params string[] types)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (oldEmail == null) throw new ArgumentNullException(nameof(oldEmail));

            ReplaceLine(CardProperty.EMAIL.ToString(), types, email, oldEmail);

            return this;
        }

        public IVCardBuilder AddPhone(string phone, params string[] types)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));

            AddLine(CardProperty.TEL, phone, types, true);

            return this;
        }

        public IVCardBuilder UpdatePhone(string oldPhone, string phone, params string[] types)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));
            if (oldPhone == null) throw new ArgumentNullException(nameof(oldPhone));

            ReplaceLine(CardProperty.TEL.ToString(), types, phone, oldPhone);

            return this;
        }

        public VCardModel Build() => new();

        #endregion

        #region Helpers

        void AddLine(CardProperty property, string value, bool canReplace)
        {
            AddLine(property.ToString(), value, new string[0], canReplace);
        }

        void AddLine(CardProperty property, string value, string[] parameters, bool canReplace)
        {
            AddLine(property.ToString(), value, parameters, canReplace);
        }

        void AddLine(string property, string value, string[] parameters, bool canReplace)
        {
            if (parameters.Any())
                VCardParser.AddLineToCard(vCard1, $"{property};{string.Join(';', parameters)}:{value}", canReplace);
            else
                VCardParser.AddLineToCard(vCard1, $"{property}:{value}", canReplace);
        }

        void ReplaceLine(string property, string[] parameters, string value, string oldValue)
        {
            if (parameters.Any())
                VCardParser.ReplaceLine(vCard1, $"{property};{string.Join(';', parameters)}:{value}", oldValue);
            else
                VCardParser.ReplaceLine(vCard1, $"{property}:{value}", oldValue);
        }

        #endregion
    }

    public interface IVCardBuilder
    {
        IVCardBuilder SetUId(string uId);
        IVCardBuilder SetName(string name);
        IVCardBuilder SetName(string familyNames, string givenNames, string additionalNames = "", string honorificPrefixes = "", string honorificSuffixes = "");
        IVCardBuilder AddPhone(string phone, params string[] types);
        IVCardBuilder UpdatePhone(string oldPhone, string phone,  params string[] types);
        IVCardBuilder AddEmail(string email, params string[] types);
        IVCardBuilder UpdateEmail(string oldEmail, string email, params string[] types);
        VCardModel Build();

    }
}
