﻿using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BrandUp.CardDav.Server.Controllers
{
    [Route("{controller}")]
    public class PrincipalController : CardDavController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [AcceptVerbs("PROPFIND")]
        [Consumes("text/xml")]
        [Route("{Name}")]
        public async Task<ActionResult<string>> PropfindAsync([FromRoute] string userName)
        {
            if (!Request.ContentType.Contains("xmls"))
                return new UnsupportedMediaTypeResult();

            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);

            var request = await reader.ReadToEndAsync();

            var xmlString = "   <?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n  " +
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

            return Content(xmlString, "text/xml");
        }
    }
}
