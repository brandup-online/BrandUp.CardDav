using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Xml;

namespace BrandUp.CardDav.Transport.Helpers
{
    internal static class HttpConvertHelper
    {
        public static HttpRequestMessage Convert(HttpMethod method, ICardDavRequest request)
        {
            MemoryStream ms = new();

            CustomSerializer.SerializeRequest(ms, request.Body);

#if DEBUG
            var debugReader = new StreamReader(ms);
            var debug = debugReader.ReadToEnd();
            ms.Position = 0;
#endif
            HttpRequestMessage httprequest = new()
            {
                Method = method,
                Content = new StreamContent(ms)
            };

            httprequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
            foreach (var header in request.Headers)
            {
                httprequest.Content.Headers.Add(header.Key, header.Value);
            }

            return httprequest;
        }
    }
}
