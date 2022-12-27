using BrandUp.CardDav.Attributes;
using BrandUp.CardDav.Server.Abstractions.Documents;

namespace BrandUp.CardDav.Server.Documents
{
    public interface IContactDocument : IDavDocument
    {
        public Guid AddressBookId { get; set; }
        [DavName("getetag")]
        public string ETag { get; set; }
        [DavName("address-data", "urn:ietf:params:xml:ns:carddav")]
        public string RawVCard { get; set; }
    }
}
