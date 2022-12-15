using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Responses;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Client
{
    public interface ICardDavClient
    {
        public Task<OptionsResponse> OptionsAsync(CancellationToken cancellationToken);
        public Task<PropfindResponse> PropfindAsync(string endpoint, PropfindRequest request, CancellationToken cancellationToken = default);
        public Task<ReportResponse> ReportAsync(string endpoint, ReportRequest request, CancellationToken cancellationToken = default);
        public Task<MkcolResponse> MkcolAsync(string endpoint, CancellationToken cancellationToken = default);
        public Task<VCardModel> GetAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> AddContactAsync(string endpoint, VCardModel vCard, CancellationToken cancellationToken);
        public Task<CarddavResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> UpdateContactAsync(string endpoint, VCardModel vCard, string ETag, CancellationToken cancellationToken);
    }
}
