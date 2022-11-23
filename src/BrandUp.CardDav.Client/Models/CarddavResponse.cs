namespace BrandUp.Carddav.Client.Models
{
    public class CarddavResponse
    {
        public IList<AddressBookResponse> addressBooks { get; set; } = new List<AddressBookResponse>();
        public IList<VCardResponse> vCards { get; set; } = new List<VCardResponse>();

        public string SyncToken { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string StatusCode { get; set; }

    }
}
