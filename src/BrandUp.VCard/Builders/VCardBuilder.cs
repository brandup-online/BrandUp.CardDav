namespace BrandUp.CardDav.VCard.Builders
{
    public class VCardBuilder : IVCardBuilder
    {
        Dictionary<CardProperty, IEnumerable<VCardLine>> propertyDictionary = new();

        public VCardBuilder(VCardVersion version = VCardVersion.VCard3)
        {
            propertyDictionary.Add(CardProperty.VERSION, new VCardLine[1] { new VCardLine { Value = $"{version.ToString().Last()}.0"} });
        }

        public VCardBuilder(string rawVCard) : this()
        {
            propertyDictionary = new(VCardParser.ParseLinesAsync(new StringReader(rawVCard), CancellationToken.None).Result);

        }

        public VCardBuilder(VCardModel vCard)
        {
            if (vCard == null)
                throw new ArgumentNullException(nameof(vCard));

            propertyDictionary = vCard.ToDictionary();
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

            AddLine(CardProperty.FN, name, null, false);
            return this;
        }

        public IVCardBuilder SetName(string familyNames, string givenNames, string additionalNames = "", string honorificPrefixes = "", string honorificSuffixes = "")
        {
            if (familyNames == null) throw new ArgumentNullException(nameof(familyNames));
            if (givenNames == null) throw new ArgumentNullException(nameof(givenNames));
            if (additionalNames == null) throw new ArgumentNullException(nameof(additionalNames));
            if (honorificPrefixes == null) throw new ArgumentNullException(nameof(honorificPrefixes));
            if (honorificSuffixes == null) throw new ArgumentNullException(nameof(honorificSuffixes));

            AddLine(CardProperty.FN, string.Join(";", familyNames, givenNames, additionalNames, honorificPrefixes, honorificSuffixes), new string[0], false);
            return this;
        }

        public IVCardBuilder SetUId(string uId)
        {
            if (uId == null) throw new ArgumentNullException(nameof(uId));

            AddLine(CardProperty.UID, uId, true);

            return this;
        }

        public IVCardBuilder AddEmail(string email, Kind? kind, params string[] types)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));

            AddLine(CardProperty.EMAIL, email, types.Select(t => $"type={t}").Append($"type={kind}").ToArray(), false);
            return this;
        }

        public IVCardBuilder UpdateEmail(string oldEmail, string email, params string[] types)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            if (oldEmail == null) throw new ArgumentNullException(nameof(oldEmail));

            ReplaceLine(CardProperty.EMAIL.ToString(), types.Select(t => $"type={t}").ToArray(), email, oldEmail);

            return this;
        }

        public IVCardBuilder AddPhone(string phone, Kind? kind, params TelType[] types)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));

            AddLine(CardProperty.TEL, phone, types.Select(t => $"type={t}").Append($"type={kind}").ToArray(), true);

            return this;
        }

        public IVCardBuilder UpdatePhone(string oldPhone, string phone, params TelType[] types)
        {
            if (phone == null) throw new ArgumentNullException(nameof(phone));
            if (oldPhone == null) throw new ArgumentNullException(nameof(oldPhone));

            ReplaceLine(CardProperty.TEL.ToString(), types.Select(t => $"type={t}").ToArray(), phone, oldPhone);

            return this;
        }

        public VCardModel Build() => new(propertyDictionary);

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
                VCardParser.AddLineToCard(propertyDictionary, $"{property};{string.Join(';', parameters)}:{value}", canReplace);
            else
                VCardParser.AddLineToCard(propertyDictionary, $"{property}:{value}", canReplace);
        }

        void ReplaceLine(string property, string[] parameters, string value, string oldValue)
        {
            if (parameters.Any())
                VCardParser.ReplaceLine(propertyDictionary, $"{property};{string.Join(';', parameters)}:{value}", oldValue);
            else
                VCardParser.ReplaceLine(propertyDictionary, $"{property}:{value}", oldValue);
        }

        #endregion
    }

    public interface IVCardBuilder
    {
        IVCardBuilder SetUId(string uId);
        IVCardBuilder SetName(string name);
        IVCardBuilder SetName(string familyNames, string givenNames, string additionalNames = "", string honorificPrefixes = "", string honorificSuffixes = "");
        IVCardBuilder AddPhone(string phone, Kind? kind, params TelType[] types);
        IVCardBuilder UpdatePhone(string oldPhone, string phone, params TelType[] types);
        IVCardBuilder AddEmail(string email, Kind? kind, params string[] types);
        IVCardBuilder UpdateEmail(string oldEmail, string email, params string[] types);
        VCardModel Build();

    }
}
