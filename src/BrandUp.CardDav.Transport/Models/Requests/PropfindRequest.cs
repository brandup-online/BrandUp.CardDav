using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Requests.Body.Propfind;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    public class PropfindRequest : ICardDavRequest
    {
        public Depth Depth => Depth.Parse(Headers["Depth"]);
        public PropfindRequest() { }

        public PropfindRequest(IDictionary<string, string> headers)
        {
            Headers = headers;
        }

        public PropfindRequest(string depth)
        {
            Headers.Add("Depth", depth);
        }

        #region Static members

        public static PropfindRequest Create(Depth depth, params IDavProperty[] properties)
        {
            return new(depth.Value) { Body = new PropBody("prop") { Properties = properties } };
        }

        public static PropfindRequest AllProp(Depth depth)
        {
            return new(depth.Value) { Body = new PropBody("allprop") { } };
        }

        #endregion

        #region ICardDavRequest members 

        public IRequestBody Body { get; init; }

        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

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
