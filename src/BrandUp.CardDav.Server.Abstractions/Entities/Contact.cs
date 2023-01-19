namespace BrandUp.CardDav.Server.Abstractions.Documents
{
    /// <summary>
    /// Contact entity 
    /// </summary>
    public class Contact : IDavDocument
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Addressbook identifier.
        /// </summary>
        public Guid AddressBookId { get; set; }

        /// <summary>
        /// Entity tag.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// VCard data in string.
        /// </summary>
        public string RawVCard { get; set; }
    }
}
