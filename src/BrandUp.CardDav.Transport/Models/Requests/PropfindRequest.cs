using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Body;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    public class PropfindRequest : ICardDavRequest, IXmlConvertable
    {
        public IRequestBody Body { get; init; }

        private string depth;

        #region Static members

        public static PropfindRequest Create(Depth depth, params Prop[] properties)
        {
            return new() { depth = depth.Value, Body = new PropBody { Properties = properties } };
        }

        #endregion

        #region IXmlConvertable region

        public string Name => "propfind";

        public string Namespace => "DAV:";

        #endregion

        #region ICardDavRequest members 

        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public HttpRequestMessage ToHttpRequest()
        {
            HttpRequestMessage request = new()
            {
                Method = new("PROPFIND"),
                Content = new StringContent(XmlSerializer.Serialize(Name, Namespace, Body))
            };

            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
            request.Content.Headers.Add("Depth", depth);
            foreach (var header in Headers)
            {
                request.Content.Headers.Add(header.Key, header.Value);
            }

            return request;
        }

        #endregion

        internal class PropBody : IRequestBody
        {
            #region IRequestBody members

            public IEnumerable<IRequestProperty> Properties { get; init; }

            #endregion

            #region IXmlConvertable region

            public string Name => "prop";

            public string Namespace => "DAV:";

            #endregion
        }
    }
}
