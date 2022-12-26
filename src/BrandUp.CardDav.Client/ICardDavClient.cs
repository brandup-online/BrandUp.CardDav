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
        //public Task<MkcolResponse> MkcolAsync(string endpoint, MkcolRequest request, CancellationToken cancellationToken = default);
        public Task<MkcolResponse> MkcolAsync(string endpoint, CancellationToken cancellationToken = default);
        public Task<VCardResponse> GetAsync(string endpoint, CancellationToken cancellationToken);
        public Task<BaseResponse> AddContactAsync(string endpoint, VCardModel vCard, CancellationToken cancellationToken);
        public Task<BaseResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken);
        public Task<BaseResponse> UpdateContactAsync(string endpoint, VCardModel vCard, string ETag, CancellationToken cancellationToken);
    }
}
