using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Example.Domain.Documents;

namespace BrandUp.CardDav.Server.Example.Mapping
{
    public static class MappingExtention
    {
        public static AddressBook ToAddressBook(this AddressBookDocument book)
        {
            return new() { Id = book.Id, Name = book.Name, UserId = book.UserId };
        }
        public static User ToUser(this UserDocument user)
        {
            return new() { Id = user.Id, Name = user.Name };
        }
        public static Contact ToContact(this ContactDocument contact)
        {
            return new() { Id = contact.Id, Name = contact.Name, AddressBookId = contact.AddressBookId, ETag = contact.ETag, RawVCard = contact.RawVCard };
        }
    }
}
