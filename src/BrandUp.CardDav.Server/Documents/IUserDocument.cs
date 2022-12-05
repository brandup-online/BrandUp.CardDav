namespace BrandUp.CardDav.Server.Documents
{
    public interface IUserDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid> AddressBooks { get; set; }
    }
}
