using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    [XmlRoot(ElementName = "mkcol", Namespace = "DAV:")]
    public class MkcolRequest : ICardDavRequest, IHttpRequestConvertable
    {
        #region ICardDavRequest members 

        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

        public IRequestBody Body { get; init; }

        #endregion

        #region IHttpRequestConvertable members 

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
