using BrandUp.Carddav.Client.Models;
using BrandUp.Carddav.Client.Models.Requests;
using BrandUp.Carddav.Client.Models.Responses;

namespace BrandUp.Carddav.Client.Client
{
    public interface ICardDavClient
    {
        public Task<CarddavResponse> OptionsAsync(CancellationToken cancellationToken);
        public Task<CarddavResponse> PropfindAsync(string endpoint, CarddavRequest request, CancellationToken cancellationToken);
        public Task<CarddavResponse> ReportAsync(string endpoint, CarddavRequest request, CancellationToken cancellationToken);
        public Task<CarddavResponse> GetAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> AddContactAsync(string endpoint, VCard vCard, CancellationToken cancellationToken);
        public Task<CarddavResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> UpdateContactAsync(string endpoint, VCard vCard, CarddavRequest request, CancellationToken cancellationToken);
    }
}
