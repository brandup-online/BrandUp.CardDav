namespace BrandUp.CardDav.Client.Options
{
    /// <summary>
    /// Contains additional options, for credentials authorizations
    /// </summary>
    public class CardDavCredentialsOptions : CardDavOptions
    {
        /// <summary>
        /// server login
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// server password
        /// </summary>
        public string Password { get; set; }
    }
}
