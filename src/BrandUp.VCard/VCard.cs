namespace BrandUp.VCard
{
    public class VCard
    {
        public string FormattedName { get; internal set; }
        public Version Version { get; internal set; }
        public string UId { get; internal set; }
        public VCardName Name { get; internal set; }
        public IList<VCardPhone> Phones { get; internal set; }
        public IList<VCardEmail> Emails { get; internal set; }
        public IDictionary<string, string> AdditionalFields { get; internal set; }

        public VCard()
        {
            Phones = new List<VCardPhone>();
            Emails = new List<VCardEmail>();
            AdditionalFields = new Dictionary<string, string>();
        }

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
        public Kind? Kind { get; set; }
        public string Phone { get; set; }
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

    public enum Version
    {
        VCard1,
        VCard2,
        VCard3,
        VCard4,
    }

}
