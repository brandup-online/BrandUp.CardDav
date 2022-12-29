using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Requests.Body.Propfind;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    /// <summary>
    /// Propfind request object
    /// </summary>
    public class PropfindRequest : ICardDavRequest, IHttpRequestConvertable
    {
        /// <summary>
        /// 
        /// </summary>
        public Depth Depth { get; }

        /// <summary>
        /// 
        /// </summary>
        public PropfindRequest() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        public PropfindRequest(IDictionary<string, string> headers)
        {
            Headers = headers;
            Depth = Depth.Parse(Headers["Depth"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth"></param>
        public PropfindRequest(string depth)
        {
            Headers.Add("Depth", depth);
        }

        #region Static members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static PropfindRequest Create(Depth depth, params IDavProperty[] properties)
        {
            return new(depth.Value) { Body = new PropBody("prop") { Properties = properties } };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static PropfindRequest AllProp(Depth depth)
        {
            return new(depth.Value) { Body = new PropBody("allprop") { } };
        }

        #endregion

        #region ICardDavRequest members 

        /// <summary>
        /// 
        /// </summary>
        public IRequestBody Body { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

        #endregion

        #region IHttpRequestConvertable members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
