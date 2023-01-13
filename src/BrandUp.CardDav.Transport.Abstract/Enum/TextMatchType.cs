namespace BrandUp.CardDav.Transport.Abstract.Enum
{
    /// <summary>
    /// Defines how text conditions should work.
    /// </summary>
    public enum TextMatchType
    {
        /// <summary>
        /// Text must be equal to match string.
        /// </summary>
        Equals,

        /// <summary>
        /// Text must contain match string.
        /// </summary>
        Contains,

        /// <summary>
        /// Text must starts with the match string.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Text must ends with the match string.
        /// </summary>
        EndsWith
    }
}
