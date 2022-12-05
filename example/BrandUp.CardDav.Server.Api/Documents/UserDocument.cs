using BrandUp.CardDav.Server.Documents;
using BrandUp.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace BrandUp.CardDav.Server.Example.Documents
{
    [Document(CollectionName = "CardDav.Users")]
    public class UserDocument : IUserDocument
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> AddressBooks { get; set; }
    }
}
