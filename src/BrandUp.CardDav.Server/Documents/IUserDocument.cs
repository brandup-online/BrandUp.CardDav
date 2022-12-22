using BrandUp.CardDav.Attributes;

namespace BrandUp.CardDav.Server.Documents
{
    public interface IUserDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        [DavName("getctag", "http://calendarserver.org/ns/")]
        public string CTag { get; set; }
    }
}
