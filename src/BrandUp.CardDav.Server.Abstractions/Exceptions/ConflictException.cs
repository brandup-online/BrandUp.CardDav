namespace BrandUp.CardDav.Server.Abstractions.Exceptions
{
    /// <summary>
    /// Throws it out if the creating collection is already contained in the database
    /// </summary>
    public class ConflictException : Exception
    {
        /// <summary>
        /// Throws if made collection already contains in database
        /// </summary>
        /// <param name="document">Collection document</param>
        /// <param name="name">Name of object</param>
        public ConflictException(IDavDocument document, string name) : base($"The collections {document.GetType().Name} already have object with name {name}")
        { }
    }
}
