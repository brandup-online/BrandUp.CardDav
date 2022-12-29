using BrandUp.CardDav.Server.Abstractions.Documents;

namespace BrandUp.CardDav.Server.Documents
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
