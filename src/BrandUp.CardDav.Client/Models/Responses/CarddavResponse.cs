namespace BrandUp.Carddav.Client.Models.Responses
{
    public class CarddavResponse
    {
        public IList<AddressBookResponse> addressBooks { get; set; } = new List<AddressBookResponse>();
        public IList<VCardResponse> vCardLinks { get; set; } = new List<VCardResponse>();
        public IList<VCard> vCards { get; set; } = new List<VCard>();

        public string SyncToken { get; set; }
        public string eTag { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string StatusCode { get; set; }

    }
}
