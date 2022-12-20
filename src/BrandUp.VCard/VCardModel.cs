namespace BrandUp.CardDav.VCard
{
    public class VCardModel
    {
        public string FormattedName { get; internal set; }
        public VCardVersion? Version { get; internal set; }
        public string UId { get; internal set; }
        public VCardName Name { get; internal set; }
        public IList<VCardPhone> Phones { get; internal set; }
        public IList<VCardEmail> Emails { get; internal set; }
        public IDictionary<string, string> AdditionalFields { get; internal set; }

        public VCardModel()
        {
            Phones = new List<VCardPhone>();
            Emails = new List<VCardEmail>();
            AdditionalFields = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            var result = "BEGIN:VCARD\r\n";

            result += ToVCardStringVersion();

            result += ToVCardStringUID();

            result += ToVCardStringName();

            result += ToVCardStringFormattedName();

            result += ToVCardStringAdditionalFields();

            result += ToVCardStringEmailes();

            result += ToVCardStringPhones();

            result += "END:VCARD\r\n";

            return result;
        }

        public string ToStringProps(IEnumerable<VCardProperty> properties)
        {
            var result = "";

            if (properties.Contains(VCardProperty.VERSION))
                result += ToVCardStringVersion();

            if (properties.Contains(VCardProperty.UID))
                result += ToVCardStringUID();

            if (properties.Contains(VCardProperty.N))
                result += ToVCardStringName();

            if (properties.Contains(VCardProperty.FN))
                result += ToVCardStringFormattedName();

            if (properties.Contains(VCardProperty.EMAIL))
                result += ToVCardStringEmailes();

            if (properties.Contains(VCardProperty.TEL))
                result += ToVCardStringPhones();

            return result;
        }

        public string ToStringProps(params VCardProperty[] property)
            => ToStringProps(property.ToArray());

        #region Helpers 

        string ToVCardStringVersion()
        {
            var version = Version switch
            {
                VCardVersion.VCard4 => "4.0",
                VCardVersion.VCard3 => "3.0",
                VCardVersion.VCard2 => "2.0",
                VCardVersion.VCard1 => "1.0",
                _ => throw new ArgumentException(),
            };
            return $"VERSION:{version}\r\n";
        }

        string ToVCardStringUID() => UId == null ? "" : "UID:" + UId + "\r\n";

        string ToVCardStringName()
        {
            return "N:" + string.Join(";", string.Join(",", Name.FamilyNames),
                                       string.Join(",", Name.GivenNames),
                                       string.Join(",", Name.AdditionalNames),
                                       string.Join(",", Name.HonorificPrefixes),
                                       string.Join(",", Name.HonorificSuffixes)) + "\r\n";

        }

        string ToVCardStringFormattedName() => $"FN:{FormattedName}\r\n";

        string ToVCardStringAdditionalFields()
        {
            string result = "";
            foreach (var item in AdditionalFields)
            {
                result += item.Key + ":" + item.Value + "\r\n";
            }
            return result;
        }

        string ToVCardStringEmailes()
        {
            string result = "";
            foreach (var item in Emails)
            {
                var emailStr = "EMAIL";

                if (item.Kind != null)
                    emailStr += $";TYPE={item.Kind}";

                if (item.Types.Any())
                    emailStr += ";" + string.Join(";", item.Types.Select(t => $"TYPE={t}").ToList());

                emailStr += $":{item.Email}\r\n";

                result += emailStr;
            }

            return result;
        }

        string ToVCardStringPhones()
        {
            string result = "";
            foreach (var item in Phones)
            {
                var phoneStr = "TEL";

                if (item.Kind != null)
                    phoneStr += $";TYPE={item.Kind}";
                if (item.Types.Any())
                    phoneStr += ";" + string.Join(";", item.Types.Select(t => $"TYPE={t}").ToList());

                phoneStr += $":{item.Phone}\r\n";

                result += phoneStr;
            }

            return result;
        }
        #endregion
    }

    public class VCardName
    {
        public IEnumerable<string> FamilyNames { get; set; }
        public IEnumerable<string> GivenNames { get; set; }
        public IEnumerable<string> AdditionalNames { get; set; }
        public IEnumerable<string> HonorificPrefixes { get; set; }
        public IEnumerable<string> HonorificSuffixes { get; set; }
    }

    public class VCardPhone
    {
        public string Phone { get; set; }
        public Kind? Kind { get; set; }
        public string[] Types { get; set; }
    }

    public class VCardEmail
    {
        public string Email { get; set; }
        public Kind? Kind { get; set; }
        public string[] Types { get; set; }
    }

    public enum Kind
    {
        Work,
        Home
    }

    public enum VCardVersion
    {
        VCard1,
        VCard2,
        VCard3,
        VCard4,
    }

}
