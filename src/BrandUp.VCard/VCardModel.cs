using System.Numerics;

namespace BrandUp.CardDav.VCard
{
    public class VCardModel
    {
        private IDictionary<CardProperty, IEnumerable<VCardLine>> vCard;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<VCardLine> this[CardProperty key] => vCard[key];

        public string FormattedName { get; private set; }
        public VCardVersion? Version { get; private set; }
        public string UId { get; private set; }
        public VCardName Name { get; private set; }
        public IList<VCardPhone> Phones { get; private set; }
        public IList<VCardEmail> Emails { get; private set; }

        internal VCardModel(IDictionary<CardProperty, IEnumerable<VCardLine>> vCard)
        {
            this.vCard = vCard ?? throw new ArgumentNullException(nameof(vCard));

            SetProperties();
        }

        public override string ToString()
        {
            var result = "BEGIN:VCARD\r\n";

            foreach (var property in vCard)
            {
                foreach (var line in property.Value)
                {
                    if (line.Parameters != null)
                    {
                        var parameters = string.Join(';', line?.Parameters.Select(p => $"{p.Key}={p.Value}"));
                        result += $"{property.Key};{parameters}:{line.Value}";
                    }
                    else
                    {
                        result += $"{property.Key}:{line.Value}";
                    }
                }
            }

            result += "END:VCARD\r\n";

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as VCardModel;
            if (other == null) return false;

            if (UId != null && other.UId != null)
                if (UId != other.UId) return false; // RFC 6450: 7.1.1. ...vCard instances for which the UID properties (Section 6.7.6) are equivalent MUST be matched.
                else return true;
            if (Version != other.Version) return false;
            if (!string.Equals(FormattedName, other.FormattedName, StringComparison.OrdinalIgnoreCase)) return false;
            if (!Name.Equals(other.Name)) return false;
            if (!Emails.SequenceEqual(other.Emails)) return false;
            if (!Phones.SequenceEqual(other.Phones)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string ToStringProps(IEnumerable<CardProperty> properties)
        {
            var result = "";

            if (properties.Contains(CardProperty.VERSION))
                result += ToVCardStringVersion();

            if (properties.Contains(CardProperty.UID))
                result += ToVCardStringUID();

            if (properties.Contains(CardProperty.N))
                result += ToVCardStringName();

            if (properties.Contains(CardProperty.FN))
                result += ToVCardStringFormattedName();

            if (properties.Contains(CardProperty.EMAIL))
                result += ToVCardStringEmailes();

            if (properties.Contains(CardProperty.TEL))
                result += ToVCardStringPhones();

            return result;
        }

        /// <summary>
        /// Converts to stringprops.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public string ToStringProps(params CardProperty[] property)
            => ToStringProps(property.ToArray());

        /// <summary>Converts to dictionary.</summary>
        /// <returns>
        ///   <br />
        /// </returns>
        public Dictionary<CardProperty, IEnumerable<VCardLine>> ToDictionary() => (Dictionary<CardProperty, IEnumerable<VCardLine>>)vCard;

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
            //foreach (var item in AdditionalFields)
            //{
            //    result += item.Key + ":" + item.Value + "\r\n";
            //}
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

        void SetProperties()
        {
            if (vCard.TryGetValue(CardProperty.FN, out var vCardLines))
                SetFormattedName(vCardLines.First());
            if (vCard.TryGetValue(CardProperty.N, out vCardLines))
                SetName(vCardLines.First());
            if (vCard.TryGetValue(CardProperty.TEL, out vCardLines))
                SetPhones(vCardLines);
            if (vCard.TryGetValue(CardProperty.EMAIL, out vCardLines))
                SetEmails(vCardLines);
            if (vCard.TryGetValue(CardProperty.UID, out vCardLines))
                SetUid(vCardLines.First());
            if (vCard.TryGetValue(CardProperty.VERSION, out vCardLines))
                SetVersion(vCardLines.First());
        }

        #endregion

        #region Propertiy initializers

        private void SetFormattedName(VCardLine line)
        {
            if (FormattedName != null)
                throw new NotSupportedException("Cannot work with vcard that have two FN property");

            if (line == null)
                throw new ArgumentNullException(nameof(line));

            var name = line.Value;
            FormattedName = name.Trim();
        }

        private void SetName(VCardLine line)
        {
            if (Name != null)
                throw new NotSupportedException("Cannot work with vcard that have two N property");

            if (line == null)
                throw new ArgumentNullException(nameof(line));

            var name = line.Value.Split(';');
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

            Name = vCardName;

        }

        private void SetPhones(IEnumerable<VCardLine> lines)
        {
            Phones = new List<VCardPhone>();

            foreach (VCardLine line in lines)
            {
                var phone = new VCardPhone();
                var types = new List<TelType>();

                phone.Phone = line.Value;
                foreach (var prop in line?.Parameters)
                {
                    if (prop.Key == CardParameter.TYPE)
                    {
                        foreach (var type in prop.Value)
                        {
                            if (string.Equals(type, "work", StringComparison.InvariantCultureIgnoreCase))
                            {
                                phone.Kind = Kind.Work;
                            }
                            else if (string.Equals(type, "home", StringComparison.InvariantCultureIgnoreCase))
                            {
                                phone.Kind = Kind.Home;
                            }
                            else
                            {
                                if (Enum.TryParse<TelType>(type, true, out var telType))
                                    types.Add(telType);
                            }
                        }
                    }
                }

                phone.Types = types.ToArray();
                Phones.Add(phone);
            }
        }

        private void SetEmails(IEnumerable<VCardLine> lines)
        {
            Emails = new List<VCardEmail>();

            foreach (VCardLine line in lines)
            {
                var email = new VCardEmail();
                var types = new List<EmailType>();

                email.Email = line.Value;
                foreach (var prop in line.Parameters)
                {
                    if (prop.Key == CardParameter.TYPE)
                    {
                        foreach (var type in prop.Value)
                        {
                            if (string.Equals(type, "work", StringComparison.InvariantCultureIgnoreCase))
                            {
                                email.Kind = Kind.Work;
                            }
                            else if (string.Equals(type, "home", StringComparison.InvariantCultureIgnoreCase))
                            {
                                email.Kind = Kind.Home;
                            }
                            else
                            {
                                if (Enum.TryParse<EmailType>(type, true, out var telType))
                                    types.Add(telType);
                            }
                        }
                    }
                }

                email.Types = types.ToArray();
                Emails.Add(email);
            }
        }

        private void SetUid(VCardLine line)
        {
            if (UId != null)
                throw new NotSupportedException("cannot work with vcard that have two UID property");

            var uid = line.Value;
            UId = uid.Trim();
        }

        private void SetVersion(VCardLine line)
        {
            if (Version != null)
                throw new NotSupportedException("cannot work with vcard that have two UID property");

            var version = line.Value.Trim();
            Version = version switch
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

    public class VCardLine
    {
        public string Value { get; set; }
        public IDictionary<CardParameter, IEnumerable<string>> Parameters { get; set; }
    }

    public class VCardName
    {
        public IEnumerable<string> FamilyNames { get; set; }
        public IEnumerable<string> GivenNames { get; set; }
        public IEnumerable<string> AdditionalNames { get; set; }
        public IEnumerable<string> HonorificPrefixes { get; set; }
        public IEnumerable<string> HonorificSuffixes { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as VCardName;
            if (other == null) return false;

            if (FamilyNames != null && other.FamilyNames != null)
                if (!FamilyNames.SequenceEqual(other.FamilyNames)) return false;

            if (GivenNames != null && other.GivenNames != null)
                if (!GivenNames.SequenceEqual(other.GivenNames)) return false;

            if (AdditionalNames != null && other.AdditionalNames != null)
                if (!AdditionalNames.SequenceEqual(other.AdditionalNames)) return false;

            if (HonorificPrefixes != null && other.HonorificPrefixes != null)
                if (!HonorificPrefixes.SequenceEqual(other.HonorificPrefixes)) return false;

            if (HonorificSuffixes != null && other.HonorificSuffixes != null)
                if (!HonorificSuffixes.SequenceEqual(other.HonorificSuffixes)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class VCardPhone
    {
        public string Phone { get; set; }
        public Kind? Kind { get; set; }
        public TelType[] Types { get; set; }

        public override int GetHashCode()
             => $"{Phone} {Kind} {string.Join(',', Types)}".GetHashCode();

        public override bool Equals(object obj)
             => GetHashCode() == obj.GetHashCode();
    }

    public class VCardEmail
    {
        public string Email { get; set; }
        public Kind? Kind { get; set; }
        public EmailType[] Types { get; set; }

        public override int GetHashCode()
             => $"{Email} {Kind} ".GetHashCode();

        public override bool Equals(object obj)
            => GetHashCode() == obj.GetHashCode();
    }
}
