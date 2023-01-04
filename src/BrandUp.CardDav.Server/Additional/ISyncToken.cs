using BrandUp.CardDav.Attributes;

namespace BrandUp.CardDav.Server.Abstractions.Additional
{
    /// <summary>
    /// Provide Sync token to context
    /// </summary>
    public interface ISyncToken
    {
        /// <summary>
        /// Syncronization token
        /// </summary>
        [DavName("sync-token", "DAV:")]
        public Uri SyncToken { get; }
    }
}
