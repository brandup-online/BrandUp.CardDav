using BrandUp.CardDav.Server.Abstractions.Additional;
using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace BrandUp.CardDav.Server.Example.Domain.Documents
{
    [Document(CollectionName = "CardDav.AddressBooks")]
    public class AddressBookDocument : IAddressBookDocument, ICTag
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public string CTag { get; set; }

        public void SetForCreation(string name, Guid userId)
        {
            Name = name;
            Id = Guid.NewGuid();
            UserId = userId;
            CTag = DateTime.UtcNow.ToString("\"yyyy-MM-dd:hh-mm-ss\"");
        }
    }
}
