namespace BrandUp.CardDav.Server.Abstractions.Documents
{
    /// <summary>
    /// Addressbook entity 
    /// </summary>
    public interface IAddressBookDocument : IDavDocument
    {
        /// <summary>
        /// User identifier
        /// </summary>
        public Guid UserId { get; set; }
    }
}
