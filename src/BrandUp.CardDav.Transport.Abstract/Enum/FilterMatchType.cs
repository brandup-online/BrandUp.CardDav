namespace BrandUp.CardDav.Transport.Abstract.Enum
{
    /// <summary>
    /// Defines how multiple conditions should work.
    /// </summary>
    public enum FilterMatchType
    {
        /// <summary>
        /// VCard must meet to all condition.
        /// </summary>
        All,

        /// <summary>
        /// VCard must meet at least one condition.
        /// </summary>
        Any
    }
}
