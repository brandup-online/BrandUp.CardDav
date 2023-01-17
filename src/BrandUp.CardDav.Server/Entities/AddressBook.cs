namespace BrandUp.CardDav.Server.Abstractions.Documents
{
    /// <summary>
    /// Addressbook entity 
    /// </summary>
    public class AddressBook : IDavDocument
    {
        /// <summary>
        /// User identifier
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
}
