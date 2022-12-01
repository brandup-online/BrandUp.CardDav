using BrandUp.CardDav.Client.Models.Responses;

namespace BrandUp.CardDav.Client.Client
{
    public interface ICardDavClient
    {
        public Task<CarddavResponse> OptionsAsync(CancellationToken cancellationToken);
        public Task<CarddavResponse> PropfindAsync(string endpoint, string xmlRequest, string depth = "0", CancellationToken cancellationToken = default);
        public Task<CarddavResponse> ReportAsync(string endpoint, string xmlRequest, string depth = "0", CancellationToken cancellationToken = default);
        public Task<CarddavResponse> GetAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> AddContactAsync(string endpoint, VCard.VCard vCard, CancellationToken cancellationToken);
        public Task<CarddavResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> UpdateContactAsync(string endpoint, VCard.VCard vCard, string ETag, CancellationToken cancellationToken);
    }
}
