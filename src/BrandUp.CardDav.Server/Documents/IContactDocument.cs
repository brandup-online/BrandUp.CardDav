using BrandUp.CardDav.Attributes;

namespace BrandUp.CardDav.Server.Documents
{
    public interface IContactDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AddressBookId { get; set; }
        [DavName("getetag")]
        public string ETag { get; set; }
        [DavName("address-data", "urn:ietf:params:xml:ns:carddav")]
        public string RawVCard { get; set; }
    }
}
