namespace BrandUp.CardDav.Server.Documents
{
    public interface IContactDocument
    {
        public Guid Id { get; set; }
        public Guid AddressBookId { get; set; }
    }
}
