using BrandUp.CardDav.Attributes;

namespace BrandUp.CardDav.Server.Abstractions.Additional
{
    public interface ICTag
    {
        [DavName("getctag", "http://calendarserver.org/ns/")]
        public string CTag { get; set; }
    }
}
