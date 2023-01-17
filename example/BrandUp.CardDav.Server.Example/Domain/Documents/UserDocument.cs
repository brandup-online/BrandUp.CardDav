using BrandUp.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace BrandUp.CardDav.Server.Example.Domain.Documents
{
    [Document(CollectionName = "CardDav.Users")]
    public class UserDocument
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Password { get; set; }
        public string Name { get; set; }
        public string CTag { get; set; }

        public void SetForCreation(string name, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            Password = password;
            CTag = DateTime.UtcNow.ToString("\"yyyy-MM-dd:hh-mm-ss\"");
        }
    }
}
