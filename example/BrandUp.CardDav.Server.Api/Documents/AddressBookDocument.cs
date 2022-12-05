using BrandUp.CardDav.Server.Documents;
using BrandUp.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace BrandUp.CardDav.Server.Example.Documents
{
    [Document(CollectionName = "CardDav.AddressBooks")]
    public class AddressBookDocument : IAddressBookDocument
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ETag { get; set; }
        public List<Guid> Contacts { get; set; }
    }
}
