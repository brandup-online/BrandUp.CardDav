using BrandUp.CardDav.Attributes;

namespace BrandUp.CardDav.Server.Abstractions.Documents
{
    /// <summary>
    /// Contact entity 
    /// </summary>
    public interface IContactDocument : IDavDocument
    {
        /// <summary>
        /// Addressbook identifier.
        /// </summary>
        public Guid AddressBookId { get; set; }
        /// <summary>
        /// Entity tag.
        /// </summary>
        [DavName("getetag")]
        public string ETag { get; set; }
        /// <summary>
        /// VCard data in string.
        /// </summary>
        [DavName("address-data", "urn:ietf:params:xml:ns:carddav")]
        public string RawVCard { get; set; }
    }
}
