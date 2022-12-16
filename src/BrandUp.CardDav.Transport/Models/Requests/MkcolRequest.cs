using BrandUp.CardDav.Transport.Models.Abstract;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    [XmlRoot(ElementName = "mkcol", Namespace = "DAV:")]
    public class MkcolRequest : ICardDavRequest
    {
        #region ICardDavRequest members 

        public IDictionary<string, string> Headers { get; init; }

        public IRequestBody Body { get; init; }

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
                Method = new("MKCOL"),
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
