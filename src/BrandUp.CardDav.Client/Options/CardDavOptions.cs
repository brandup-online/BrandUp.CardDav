namespace BrandUp.CardDav.Client.Options
{
    /// <summary>
    /// Contains options for <see cref="CardDavClient" />
    /// </summary>
    public abstract class CardDavOptions
    {
        /// <summary>
        /// Base URL for client
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
