using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Transport.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class PrincipalController : CardDavController
    {
        [CardDavPropfind("{Name}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Task<ActionResult<string>> PropfindAsync([FromRoute] string userName, PropfindRequest request)
        {
            if (!Request.ContentType.Contains("xml"))
                return Task.FromResult((ActionResult<string>)new UnsupportedMediaTypeResult());

            var xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n  " +
                " <D:multistatus xmlns:D=\"DAV:\">\r\n   " +
                "  <D:response>\r\n      " +
                " <D:href>http://www.example.com/papers/</D:href>\r\n     " +
                "  <D:propstat>\r\n   " +
                "      <D:prop>\r\n        " +
                "     </D:prop>\r\n      " +
                "   <D:status>HTTP/1.1 200 OK</D:status>\r\n     " +
                "  </D:propstat>\r\n   " +
                "  </D:response>\r\n " +
                "  </D:multistatus>";

            return Task.FromResult((ActionResult<string>)Content(xmlString, "text/xml"));
        }
    }
}
