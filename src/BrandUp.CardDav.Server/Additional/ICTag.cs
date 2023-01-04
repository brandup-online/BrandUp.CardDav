using BrandUp.CardDav.Attributes;

namespace BrandUp.CardDav.Server.Abstractions.Additional
{
    /// <summary>
    /// Provide CTag property for context
    /// </summary>
    public interface ICTag
    {
        /// <summary>
        /// </summary>
        [DavName("getctag", "http://calendarserver.org/ns/")]
        public string CTag { get; set; }
    }
}
