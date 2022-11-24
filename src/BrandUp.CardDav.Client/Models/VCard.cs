namespace BrandUp.Carddav.Client.Models
{
    public class VCard
    {
        private string rawView;

        public string Raw => rawView;
        public string FullName { get; set; }
        public string UId { get; set; }
        public VCardName Name { get; set; }
        public List<VCardPhone> Phones { get; set; }
        public List<VCardEmail> Emails { get; set; }

        public VCard(string vcard)
        {
            rawView = vcard ?? throw new ArgumentNullException(nameof(vcard));

            Phones = new();
            Emails = new();
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
    }

    public class VCardEmail
    {
        public Kind? Kind { get; set; }
        public string Email { get; set; }
    }

    public enum Kind
    {
        Work,
        Home
    }
}
