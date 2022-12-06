namespace BrandUp.CardDav.Server.Documents
{
    public interface IAddressBookDocument
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ETag { get; set; }
    }
}
