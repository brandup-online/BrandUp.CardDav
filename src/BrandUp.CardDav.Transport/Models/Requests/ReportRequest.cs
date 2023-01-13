using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Transport.Helpers;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using BrandUp.CardDav.Transport.Models.Requests.Body.Report;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    /// <summary>
    /// Report request object
    /// </summary>
    public class ReportRequest : ICardDavRequest, IHttpRequestConvertable
    {
        /// <summary>
        /// 
        /// </summary>
        public ReportRequest()
        {
            Headers.Add("Depth", Depth.One.Value);
        }

        #region Static members

        /// <summary>
        /// Creates Addressbook-query. <see href="https://www.rfc-editor.org/rfc/rfc6352.html#section-10.3/" />
        /// </summary>
        /// <param name="propRequest"></param>
        /// <param name="addressData"></param>
        /// <param name="filter"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static ReportRequest CreateQuery(PropList propRequest, AddressData addressData, FilterBody filter, int limit = 0)
            => new ReportRequest()
            {
                Body = new AddresbookQueryBody()
                {
                    PropList = propRequest.Properties.Append(addressData),
                    Filter = filter,
                    Limit = limit
                }
            };

        /// <summary>
        /// Creates Multiget-query. <see href="https://www.rfc-editor.org/rfc/rfc6352.html#section-10.7"/>
        /// </summary>
        /// <param name="propRequest"></param>
        /// <param name="addressData"></param>
        /// <param name="endpoints"></param>
        /// <returns></returns>
        public static ReportRequest CreateMultiget(PropList propRequest, AddressData addressData, params string[] endpoints)
            => new ReportRequest()
            {
                Body = new MultigetBody()
                {
                    PropList = propRequest.Properties.Append(addressData),
                    VCardEndpoints = endpoints
                }
            };

        #endregion

        #region ICardDavRequest members

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

        IRequestBody ICardDavRequest.Body => Body;

        /// <summary>
        /// 
        /// </summary>
        public IRequestBody Body { get; init; }

        #endregion

        #region IHttpRequestConvertable members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HttpRequestMessage ToHttpRequest() => HttpConvertHelper.Convert(new("REPORT"), this);

        #endregion
    }
}
