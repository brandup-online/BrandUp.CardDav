using BrandUp.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace BrandUp.CardDav.Server.Example.Domain.Documents
{
    [Document(CollectionName = "CardDav.Contacts")]
    public class ContactDocument
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AddressBookId { get; set; }
        public string ETag { get; set; }
        public string RawVCard { get; set; }

        public void SetForCreation(string name, Guid addressBookId, string rawVCard)
        {
            Name = name;
            Id = Guid.NewGuid();
            AddressBookId = addressBookId;
            ETag = $"\"{Id.GetHashCode()}-{DateTime.UtcNow.GetHashCode()}\"";
            RawVCard = rawVCard;
        }

        public void PreUpdate()
        {
            ETag = $"{Id.GetHashCode()}-{DateTime.UtcNow.GetHashCode()}";
        }
    }
}
