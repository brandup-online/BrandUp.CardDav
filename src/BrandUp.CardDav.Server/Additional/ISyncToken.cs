using BrandUp.CardDav.Attributes;

namespace BrandUp.CardDav.Server.Abstractions.Additional
{
    public interface ISyncToken
    {
        [DavName("sync-token", "DAV:")]
        public Uri SyncToken { get; }
    }
}
