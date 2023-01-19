namespace BrandUp.CardDav.Server.Abstractions.Documents
{
    /// <summary>
    /// User entity
    /// </summary>
    public class User : IDavDocument
    {
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
