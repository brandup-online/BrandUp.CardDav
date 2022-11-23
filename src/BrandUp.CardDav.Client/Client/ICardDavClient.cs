using BrandUp.Carddav.Client.Models;

namespace BrandUp.Carddav.Client.Client
{
    public interface ICardDavClient
    {
        public Task<CarddavResponse> OptionsAsync(CancellationToken cancellationToken);
        public Task<CarddavResponse> PropfindAsync(string endpoint, CarddavRequest request, CancellationToken cancellationToken);
        public Task<CarddavResponse> ReportAsync(string endpoint, CarddavRequest request, CancellationToken cancellationToken);
        public Task<CarddavResponse> GetAsync(string endpoint, CancellationToken cancellationToken);
        public Task<CarddavResponse> AddContactAsync(string endpoint, string vCard, CancellationToken cancellationToken);
        public Task<CarddavResponse> DeleteContactAsync(string endpoint, string vCard, CancellationToken cancellationToken);
        public Task<CarddavResponse> UpdateContactAsync(string endpoint, string vCard, CancellationToken cancellationToken);
    }
}
