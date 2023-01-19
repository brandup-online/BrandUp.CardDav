using BrandUp.CardDav.Attributes;

namespace BrandUp.CardDav.Server.Abstractions.Additional
{
    /// <summary>
    /// Provide CTag property for context
    /// </summary>
    public class CTag
    {
        /// <summary>
        /// </summary>
        [DavName("getctag", "http://calendarserver.org/ns/")]
        public string Ctag { get; set; }
    }
}
