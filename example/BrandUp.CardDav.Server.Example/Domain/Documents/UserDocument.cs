using BrandUp.CardDav.Server.Documents;
using BrandUp.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace BrandUp.CardDav.Server.Example.Domain.Documents
{
    [Document(CollectionName = "CardDav.Users")]
    public class UserDocument : IUserDocument
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CTag { get; set; }

        public void SetDorCreatiion(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            CTag = CTag = DateTime.UtcNow.ToString("\"yyyy-MM-dd:hh-mm-ss\""); ;
        }
    }
}
