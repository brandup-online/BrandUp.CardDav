using BrandUp.CardDav.Transport.Abstract.Requests;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    /// <summary>
    /// Mckol request object
    /// </summary>
    [XmlRoot(ElementName = "mkcol", Namespace = "DAV:")]
    public class MkcolRequest : ICardDavRequest, IHttpRequestConvertable
    {
        #region ICardDavRequest members 

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

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
        public HttpRequestMessage ToHttpRequest()
        {
            HttpRequestMessage request = new()
            {
                Method = new("MKCOL")
            };

            foreach (var header in Headers)
            {
                request.Content.Headers.Add(header.Key, header.Value);
            }

            return request;
        }

        #endregion
    }
}
