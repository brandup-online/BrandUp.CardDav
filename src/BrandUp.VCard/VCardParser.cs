using System.Text.RegularExpressions;

namespace BrandUp.CardDav.VCard
{
    public static class VCardParser
    {
        readonly static Dictionary<Regex, Action<VCardModel, string>> RegexDictionary;
        static VCardParser()
        {
            RegexDictionary = new()
            {
                { new(@"^FN:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddFormattedName},
                { new(@"^N:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddName},
                { new(@".*TEL.*:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddPhone},
                { new(@".*EMAIL.*:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddEmail},
                { new(@"^UID:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddUid},
                { new(@"^VERSION:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddVersion},
            };
        }

        public static async Task<VCardModel> ParseAsync(Stream responseStream, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(responseStream);

            return await ParseAsync(reader, cancellationToken);
        }

        public static async Task<VCardModel> ParseAsync(string vCardRaw, CancellationToken cancellationToken)
            => await ParseAsync(new StringReader(vCardRaw), cancellationToken);

        public static VCardModel Parse(string vCardRaw)
            => ParseAsync(new StringReader(vCardRaw), CancellationToken.None).Result;

        public static VCardModel Parse(Stream vCardStream)
            => ParseAsync(vCardStream, CancellationToken.None).Result;

        public static bool TryParse(string vCardRaw, out VCardModel vCard)
        {
            try
            {
                vCard = ParseAsync(new StringReader(vCardRaw), CancellationToken.None).Result;

                return true;
            }
            catch
            {
                vCard = null;

                return false;
            }
        }


        #region Helpers 

        private static async Task<VCardModel> ParseAsync(TextReader reader, CancellationToken cancellationToken)
        {
            VCardModel vCard = new();

            bool firstline = true;
            while (true)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (line == null)
                    break;

                if (firstline)
                {
                    firstline = false;
                    if (string.Equals(line, "BEGIN:VCARD", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    //else
                    //{
                    //    throw new ArgumentException("This is not VCard");
                    //}
                }

                if (string.Equals(line, "END:VCARD", StringComparison.InvariantCultureIgnoreCase))
                {
                    line = await reader.ReadLineAsync(cancellationToken);
                    if (line == null)
                        return vCard;
                    else if (string.Equals(line, "BEGIN:VCARD", StringComparison.InvariantCultureIgnoreCase))
                        throw new NotSupportedException("vCard with some contacts in one file.");
                }

                bool flag = true;
                foreach (var pair in RegexDictionary)
                {
                    var nameMatch = pair.Key.Match(line);
                    if (nameMatch.Success)
                    {
                        pair.Value(vCard, line);
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    var split = line.Split(':');
                    if (split.Length == 2)
                        vCard.AdditionalFields.Add(split[0], split[1]);
                }

            }
            return vCard;
        }

        #endregion

        #region Dictionary actions

        private static void AddFormattedName(VCardModel vCard, string line)
        {
            if (vCard.FormattedName != null)
                throw new NotSupportedException("Cannot work with vcard that have two FN property");

            var name = line.Split(':')[1];
            vCard.FormattedName = name.Trim();
        }


        private static void AddName(VCardModel vCard, string line)
        {
            if (vCard.Name != null)
                throw new NotSupportedException("Cannot work with vcard that have two N property");

            var name = line.Replace("N:", "").Split(';');
            VCardName vCardName = new();

            if (name[0] != "")
                vCardName.FamilyNames = name[0].Split(',');
            else vCardName.FamilyNames = new List<string>();

            if (name[1] != "")
                vCardName.GivenNames = name[1].Split(',');
            else vCardName.GivenNames = new List<string>();

            if (name[2] != "")
                vCardName.AdditionalNames = name[2].Split(',');
            else vCardName.AdditionalNames = new List<string>();

            if (name[3] != "")
                vCardName.HonorificPrefixes = name[3].Split(',');
            else vCardName.HonorificPrefixes = new List<string>();

            if (name[4] != "")
                vCardName.HonorificSuffixes = name[4].Split(',');
            else vCardName.HonorificSuffixes = new List<string>();

            vCard.Name = vCardName;

        }

        private static void AddPhone(VCardModel vCard, string line)
        {
            var phone = new VCardPhone();
            var phoneArr = line.Split(':');
            phone.Phone = phoneArr[1].Trim();
            List<string> additionalTypes = new();

            var phoneProperties = phoneArr[0].Split(";");
            if (phoneProperties.Length > 1)
                foreach (var property in phoneProperties)
                {
                    if (property.Contains("type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var kind = property.Split("=")[1].Trim();

                        if (string.Equals(kind, "work", StringComparison.InvariantCultureIgnoreCase))
                        {
                            phone.Kind = Kind.Work;
                        }
                        else if (string.Equals(kind, "home", StringComparison.InvariantCultureIgnoreCase))
                        {
                            phone.Kind = Kind.Home;
                        }
                        else
                        {
                            additionalTypes.Add(kind);
                        }
                    }
                }

            phone.Types = additionalTypes.ToArray();
            vCard.Phones.Add(phone);
        }

        private static void AddEmail(VCardModel vCard, string line)
        {
            var email = new VCardEmail();
            var emailArr = line.Split(':');
            email.Email = emailArr[1].Trim();
            List<string> additionalTypes = new();

            var emailProperties = emailArr[0].Split(";");
            if (emailProperties.Length > 1)
                foreach (var property in emailProperties)
                {
                    if (property.Contains("type", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var kind = property.Split("=")[1].Trim();

                        if (string.Equals(kind, "work", StringComparison.InvariantCultureIgnoreCase))
                        {
                            email.Kind = Kind.Work;
                        }
                        else if (string.Equals(kind, "home", StringComparison.InvariantCultureIgnoreCase))
                        {
                            email.Kind = Kind.Home;
                        }
                        else
                        {
                            additionalTypes.Add(kind);
                        }
                    }
                }

            email.Types = additionalTypes.ToArray();
            vCard.Emails.Add(email);
        }

        private static void AddUid(VCardModel vCard, string line)
        {
            if (vCard.UId != null)
                throw new NotSupportedException("cannot work with vcard that have two UID property");

            var uid = line.Replace("UID:", "");
            vCard.UId = uid.Trim();
        }

        private static void AddVersion(VCardModel vCard, string line)
        {
            if (vCard.Version != null)
                throw new NotSupportedException("cannot work with vcard that have two UID property");

            var uid = line.Replace("VERSION:", "");
            vCard.Version = uid.Trim() switch
            {
                "1.0" => VCardVersion.VCard1,
                "2.0" => VCardVersion.VCard2,
                "3.0" => VCardVersion.VCard3,
                "4.0" => VCardVersion.VCard4,
                _ => throw new ArgumentException()
            };
        }

        #endregion
    }
}
