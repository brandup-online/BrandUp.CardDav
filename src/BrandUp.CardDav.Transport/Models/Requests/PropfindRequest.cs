using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Body;
using BrandUp.CardDav.Transport.Models.Headers;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests
{

    public class PropfindRequest : ICardDavRequest
    {
        public IRequestBody Body { get; set; }
        public PropfindRequest() { }
        public PropfindRequest(IDictionary<string, string> headers) { }
        public PropfindRequest(string depth)
        {
            Headers.Add("Depth", depth);
        }

        #region Static members

        public static PropfindRequest Create(Depth depth, params Prop[] properties)
        {
            return new(depth.Value) { Body = new PropBody { Properties = properties } };
        }

        public static PropfindRequest AllProp(Depth depth)
        {
            return new(depth.Value) { Body = new AllProp() };
        }

        #endregion

        #region ICardDavRequest members 

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

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
                Method = new("PROPFIND"),
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
