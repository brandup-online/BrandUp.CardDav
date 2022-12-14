using BrandUp.CardDav.Server.Abstractions.Additional;
using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace BrandUp.CardDav.Server.Example.Domain.Documents
{
    [Document(CollectionName = "CardDav.Users")]
    public class UserDocument : IUserDocument, ICTag
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Password { get; set; }
        public string Name { get; set; }
        public string CTag { get; set; }

        public void SetForCreatiion(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            CTag = DateTime.UtcNow.ToString("\"yyyy-MM-dd:hh-mm-ss\"");
        }
    }
}
