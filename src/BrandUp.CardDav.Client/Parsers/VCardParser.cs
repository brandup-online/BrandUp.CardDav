using BrandUp.Carddav.Client.Models;
using System.Text.RegularExpressions;

namespace BrandUp.Carddav.Client.Parsers
{
    internal static class VCardParser
    {
        readonly static Dictionary<Regex, Action<VCard, string>> RegexDictionary;
        static VCardParser()
        {
            RegexDictionary = new()
            {
                { new(@"^FN:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddFullName},
                { new(@"^N:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddPartName},
                { new(@".*TEL.*:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddPhone},
                { new(@".*EMAIL.*:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddEmail},
                { new(@"^UID:(.+)$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled), AddUid},
            };

        }

        private static bool hasName = false;
        private static bool hasFullName = false;
        private static bool hasUid = false;

        public static async Task<VCard> ParseAsync(Stream responseStream, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(responseStream);
            VCard vCard = new(reader.ReadToEnd());

            reader.BaseStream.Position = 0;

            return await ParseAsync(reader, vCard, cancellationToken);
        }

        public static async Task<VCard> ParseAsync(string vCardRaw, CancellationToken cancellationToken)
        {
            VCard vCard = new(vCardRaw);

            return await ParseAsync(new StringReader(vCardRaw), vCard, cancellationToken);
        }

        public static VCard Parse(string vCardRaw)
        {
            VCard vCard = new(vCardRaw);

            return ParseAsync(new StringReader(vCardRaw), vCard, CancellationToken.None).Result;
        }

        #region Helpers 

        private static async Task<VCard> ParseAsync(TextReader reader, VCard vCard, CancellationToken cancellationToken)
        {
            while (true)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (line == null)
                    break;

                if (string.Equals(line, "BEGIN:VCARD", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                else if (string.Equals(line, "END:VCARD", StringComparison.InvariantCultureIgnoreCase))
                {
                    line = await reader.ReadLineAsync(cancellationToken);
                    if (line == null)
                        return vCard;
                    else if (string.Equals(line, "BEGIN:VCARD", StringComparison.InvariantCultureIgnoreCase))
                        throw new NotSupportedException("vCard with some contacts in one file.");
                }

                foreach (var pair in RegexDictionary)
                {
                    var nameMatch = pair.Key.Match(line);
                    if (nameMatch.Success)
                    {
                        pair.Value(vCard, line);
                        break;
                    }
                    else continue;
                }
            }
            return vCard;
        }

        #endregion

        #region Dictionary actions

        private static void AddFullName(VCard vCard, string line)
        {
            if (hasFullName)
                throw new NotSupportedException("cannot work with vcard that have two FN property");
            var name = line.Split(':')[1];
            vCard.FullName = name.Trim();
        }


        private static void AddPartName(VCard vCard, string line)
        {
            if (hasName)
                throw new NotSupportedException("cannot work with vcard that have two N property");

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

        private static void AddPhone(VCard vCard, string line)
        {
            var phone = new VCardPhone();
            var phoneArr = line.Split(':');
            phone.Phone = phoneArr[1].Trim();

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
                            break;
                        }
                        else if (string.Equals(kind, "home", StringComparison.InvariantCultureIgnoreCase))
                        {
                            phone.Kind = Kind.Home;
                            break;
                        }
                    }
                }

            vCard.Phones.Add(phone);
        }

        private static void AddEmail(VCard vCard, string line)
        {
            var email = new VCardEmail();
            var emailArr = line.Split(':');
            email.Email = emailArr[1].Trim();

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
                            break;
                        }
                        else if (string.Equals(kind, "home", StringComparison.InvariantCultureIgnoreCase))
                        {
                            email.Kind = Kind.Home;
                            break;
                        }
                    }
                }

            vCard.Emails.Add(email);
        }

        private static void AddUid(VCard vCard, string line)
        {
            if (hasUid)
                throw new NotSupportedException("cannot work with vcard that have two UID property");

            var uid = line.Replace("UID:", "");
            vCard.UId = uid.Trim();
        }

        #endregion
    }
}
