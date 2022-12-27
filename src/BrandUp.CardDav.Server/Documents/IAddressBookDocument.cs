using BrandUp.CardDav.Server.Abstractions.Documents;

namespace BrandUp.CardDav.Server.Documents
{
    public interface IAddressBookDocument : IDavDocument
    {
        public Guid UserId { get; set; }
    }
}
