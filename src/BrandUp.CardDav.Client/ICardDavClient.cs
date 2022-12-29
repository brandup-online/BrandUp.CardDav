using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Responses;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Client
{
    /// <summary>
    /// Client for cardDav requests 
    /// </summary>
    public interface ICardDavClient
    {
        /// <summary>
        /// Request a options from server. Executes to base URL. 
        /// </summary>
        /// <returns><see cref="OptionsResponse"/></returns>
        public Task<OptionsResponse> OptionsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a propfind request to server
        /// </summary>
        /// <param name="endpoint">Server endpoint</param>
        /// <param name="request">Propfind request <see cref="PropfindRequest"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="PropfindResponse"/></returns>
        public Task<PropfindResponse> PropfindAsync(string endpoint, PropfindRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a report request to server
        /// </summary>
        /// <param name="endpoint">Server endpoint</param>
        /// <param name="request">Report request <see cref="PropfindRequest"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="ReportResponse"/></returns>
        public Task<ReportResponse> ReportAsync(string endpoint, ReportRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a mkcol request to server
        /// </summary>
        /// <param name="endpoint">Server endpoint</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<MkcolResponse> MkcolAsync(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a VCard from server
        /// </summary>
        /// <param name="endpoint">VCard resource endpoint</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="VCardResponse"/></returns>
        public Task<VCardResponse> GetAsync(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds contact to a VCard collections
        /// </summary>
        /// <param name="endpoint">VCard resource endpoint</param>
        /// <param name="vCard">contact <see cref="VCardModel"/></param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="BaseResponse"/></returns>
        public Task<BaseResponse> AddContactAsync(string endpoint, VCardModel vCard, CancellationToken cancellationToken = default);


        /// <summary>
        /// Delete contact from a VCard collections
        /// </summary>
        /// <param name="endpoint">VCard resource endpoint</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="BaseResponse"/></returns>
        public Task<BaseResponse> DeleteContactAsync(string endpoint, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates contact to a VCard collections
        /// </summary>
        /// <param name="endpoint">VCard resource endpoint</param>
        /// <param name="vCard">contact to replace <see cref="VCardModel"/></param>
        /// <param name="ETag">Entity tag of contact</param>
        /// <param name="cancellationToken"></param>
        /// <returns><see cref="BaseResponse"/></returns>
        public Task<BaseResponse> UpdateContactAsync(string endpoint, VCardModel vCard, string ETag, CancellationToken cancellationToken = default);
    }
}
