using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Properties.Filters;
using BrandUp.CardDav.Transport.Models.Requests.Body.Report;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    public class ReportRequest : ICardDavRequest, IHttpRequestConvertable
    {
        public ReportRequest() { }

        public ReportRequest(Depth depth)
        {
            Headers.Add("Depth", depth.Value);
        }

        #region Static members

        public static ReportRequest CreateQuery(Depth depth, PropList propRequest, FilterBody filter, int limit = 0)
            => new ReportRequest(depth)
            {
                Body = new AddresbookQueryBody()
                {
                    PropList = propRequest.Properties,
                    Filter = filter,
                    Limit = limit
                }
            };

        public static ReportRequest CreateMultiget(Depth depth, PropList propRequest, params string[] endpoints)
            => new ReportRequest(depth)
            {
                Body = new MultigetBody()
                {
                    PropList = new List<IDavProperty>(propRequest.Properties),
                    VCardEndpoints = endpoints
                }
            };

        #endregion

        #region ICardDavRequest members

        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

        IRequestBody ICardDavRequest.Body => Body;

        public IReportBody Body { get; init; }

        #endregion

        #region IHttpRequestConvertable members

        public HttpRequestMessage ToHttpRequest()
        {
            var serializer = new XmlSerializer(Body.GetType());
            var ms = new MemoryStream();

            serializer.Serialize(ms, Body);
            ms.Position = 0;

#if DEBUG
            var debugReader = new StreamReader(ms);
            var debug = debugReader.ReadToEnd();
            ms.Position = 0;
#endif
            HttpRequestMessage request = new()
            {
                Method = new("REPORT"),
                Content = new StreamContent(ms)
            };

            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
            foreach (var header in Headers)
            {
                request.Content.Headers.Add(header.Key, header.Value);
            }

            return request;
        }

        #endregion
    }
}
