namespace BrandUp.CardDav.VCard
{
    /// <summary>
    /// 
    /// </summary>
    public class VCardModel
    {
        private IDictionary<CardProperty, PropertyModel> vCard;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public PropertyModel this[CardProperty key] => vCard[key];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(CardProperty key, out PropertyModel value)
        {
            return vCard.TryGetValue(key, out value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string FormattedName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public VCardVersion? Version { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string UId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public VCardName Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<VCardPhone> Phones { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<VCardEmail> Emails { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public VCardModel(VCardVersion version = VCardVersion.VCard3)
        {
            vCard = new Dictionary<CardProperty, PropertyModel>();
            AddPropperty(CardProperty.VERSION, $"{version.ToString().Last()}.0");

            SetProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vCard"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VCardModel(string vCard)
        {
            if (vCard == null)
                throw new ArgumentNullException(nameof(vCard));

            using var reader = new StringReader(vCard);
            this.vCard = VCardParser.Parse(reader);

            SetProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vCard"></param>
        /// <param name="close"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VCardModel(Stream vCard, bool close = false)
        {
            if (vCard == null)
                throw new ArgumentNullException(nameof(vCard));

            using var reader = new StreamReader(vCard);
            this.vCard = VCardParser.Parse(reader);

            if (close)
                vCard.Dispose();

            SetProperties();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardProperty"></param>
        /// <param name="value"></param>
        /// <param name="vCardParameters"></param>
        public void AddPropperty(CardProperty cardProperty, string value, params VCardParameter[] vCardParameters)
        {
            if (!vCard.TryAdd(cardProperty, new PropertyModel(new VCardLine() { Value = value, Parameters = vCardParameters })))
            {
                vCard[cardProperty].AddLine(new VCardLine() { Value = value, Parameters = vCardParameters });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = "BEGIN:VCARD\r\n";

            result += ToStringProps(vCard.Keys);

            result += "END:VCARD\r\n";

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as VCardModel;
            if (other == null) return false;

            // RFC 6450: 7.1.1. ...vCard instances for which the UID properties (Section 6.7.6) are equivalent MUST be matched.
            if (vCard.TryGetValue(CardProperty.UID, out var lines))
            {
                if (other[CardProperty.UID] != null)
                {
                    var uid1 = lines.Single();
                    var uid2 = other[CardProperty.UID].Single();
                    return uid1.Value.Equals(uid2.Value, StringComparison.InvariantCulture);
                }
            }

            foreach (var pair in vCard)
            {
                var otherValue = other[pair.Key];
                if (otherValue == null)
                    return false;

                if (!pair.Value.Equals(otherValue))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public string ToStringProps(IEnumerable<CardProperty> properties)
        {
            var result = "";
            foreach (var property in properties)
            {
                if (vCard.TryGetValue(property, out var propValues))
                {
                    foreach (var line in propValues)
                        if (line.Parameters.Any())
                            result += $"{property};{string.Join(';', line.Parameters.Select(_ => $"{_.Parameter}={_.Value}"))}:{line.Value}\r\n";
                        else
                            result += $"{property}:{line.Value}\r\n";
                }
            }

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

        void SetProperties()
        {
            if (vCard.TryGetValue(CardProperty.FN, out var propValues))
                FormattedName = propValues.Single().Value;
            if (vCard.TryGetValue(CardProperty.VERSION, out propValues))
                Version = Enum.Parse<VCardVersion>($"VCard{propValues.Single().Value.First()}");
            if (vCard.TryGetValue(CardProperty.UID, out propValues))
                UId = propValues.Single().Value;
            if (vCard.TryGetValue(CardProperty.N, out propValues))
                Name = new VCardName(propValues.Single().Value);
            if (vCard.TryGetValue(CardProperty.EMAIL, out propValues))
                Emails = propValues.Select(l => new VCardEmail(l));
            if (vCard.TryGetValue(CardProperty.TEL, out propValues))
                Phones = propValues.Select(l => new VCardPhone(l));
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class VCardPhone
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VCardPhone(VCardLine line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            Phone = line.Value;
            var types = new List<TelType>();
            foreach (var param in line.Parameters)
            {
                if (param.Parameter == CardParameter.TYPE)
                    if (Enum.TryParse<Kind>(param.Value, true, out var kind))
                    {
                        Kind = kind;
                    }
                    else
                        types.Add(Enum.Parse<TelType>(param.Value, true));
            }

            Types = types.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public VCardPhone()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Kind? Kind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TelType[] Types { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VCardEmail
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VCardEmail(VCardLine line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            Email = line.Value;
            var types = new List<EmailType>();
            foreach (var param in line.Parameters)
            {
                if (param.Parameter == CardParameter.TYPE)
                    if (Enum.TryParse<Kind>(param.Value, true, out var kind))
                    {
                        Kind = kind;
                    }
                    else types.Add(Enum.Parse<EmailType>(param.Value, true));
            }

            Types = types.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public VCardEmail()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Kind? Kind { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EmailType[] Types { get; set; }
    }
}
