using BrandUp.CardDav.Client.Models;

namespace BrandUp.CardDav.Client.Client
{
    public interface ICardDavClient
    {
        public Task<string> OptionsAsync(CancellationToken cancellationToken);
        public Task<string> PropfindAsync(string endpoint, CarddavRequest request, CancellationToken cancellationToken);
        public Task<string> ReportAsync(string endpoint, CarddavRequest request, CancellationToken cancellationToken);
        public Task<string> GetAsync(string endpoint, CancellationToken cancellationToken);
        public Task<string> AddContactAsync(string endpoint, string vCard, CancellationToken cancellationToken);
        public Task<string> DeleteContactAsync(string endpoint, string vCard, CancellationToken cancellationToken);
        public Task<string> UpdateContactAsync(string endpoint, string vCard, CancellationToken cancellationToken);
    }
}
