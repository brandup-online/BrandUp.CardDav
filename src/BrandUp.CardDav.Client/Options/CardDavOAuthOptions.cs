namespace BrandUp.CardDav.Client.Options
{
    /// <summary>
    /// Contains additional options, for OAuth Authorizations 
    /// </summary>
    public class CardDavOAuthOptions : CardDavOptions
    {
        /// <summary>
        /// OAuth access token
        /// </summary>
        public string AccessToken { get; set; }
    }
}
