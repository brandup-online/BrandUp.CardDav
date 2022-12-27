namespace BrandUp.CardDav.Server.Abstractions.Documents
{
    public interface IDavDocument
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
