namespace BrandUp.Carddav.Client.Models.Responses
{
    public class CarddavResponse
    {
        public IList<AddressBookResponse> AddressBooks { get; set; } = new List<AddressBookResponse>();
        public IList<ResourceResponse> ResourceEndpoints { get; set; } = new List<ResourceResponse>();
        public IList<VCardResponse> VCardResponse { get; set; } = new List<VCardResponse>();

        public string SyncToken { get; set; }

        public string ETag { get; set; }

        public bool IsSuccess { get; set; } = true;

        public string StatusCode { get; set; }

        public string RawXml { get; set; }

    }
}
