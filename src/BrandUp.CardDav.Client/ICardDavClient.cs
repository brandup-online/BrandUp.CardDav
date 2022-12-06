using BrandUp.CardDav.Client.Models.Responses;
using BrandUp.VCard;

namespace BrandUp.CardDav.Client
{
    public interface ICardDavClient
    {
        public Task<OptionsResponse> OptionsAsync(CancellationToken cancellationToken);
        public Task<CarddavResponse> PropfindAsync(string endpoint, string xmlRequest, string depth = "0", CancellationToken cancellationToken = default);
        public Task<CarddavResponse> ReportAsync(string endpoint, string xmlRequest, string depth = "0", CancellationToken cancellationToken = default);
        public Task<VCardModel> GetAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> AddContactAsync(string endpoint, VCardModel vCard, CancellationToken cancellationToken);
        public Task<CarddavResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> UpdateContactAsync(string endpoint, VCardModel vCard, string ETag, CancellationToken cancellationToken);
    }
}
