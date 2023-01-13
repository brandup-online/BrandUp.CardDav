using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Transport.Helpers;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Requests.Body.Propfind;

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
        /// Static constructor. 
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static PropfindRequest Create(Depth depth, params IDavProperty[] properties)
        {
            return new(depth.Value) { Body = new PropBody("prop") { Properties = properties } };
        }

        /// <summary>
        /// Static constructor. Request all properties that server have.
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
        /// Request body 
        /// </summary>
        public IRequestBody Body { get; init; }

        /// <summary>
        /// Reequest headers
        /// </summary>
        public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

        #endregion

        #region IHttpRequestConvertable members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public HttpRequestMessage ToHttpRequest() => HttpConvertHelper.Convert(new("PROPFIND"), this);

        #endregion
    }
}
