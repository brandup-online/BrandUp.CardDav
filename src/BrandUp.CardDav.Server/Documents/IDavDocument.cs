namespace BrandUp.CardDav.Server.Abstractions.Documents
{
    /// <summary>
    /// Interface of dav entity 
    /// </summary>
    public interface IDavDocument
    {
        /// <summary>
        /// Entity identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Entity name
        /// </summary>
        public string Name { get; set; }
    }
}
