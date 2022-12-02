using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BrandUp.CardDav.Server.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrincipalController : ControllerBase
    {
        [HttpGet]
        public Task<ActionResult> GetAsync()
        {
            return Task.FromResult((ActionResult)BadRequest());
        }

        [HttpOptions]
        public Task<ActionResult> OptionsAsync()
        {
            var allowValues = new List<string>() { "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "COPY",
                "MOVE", "MKCOL", "PROPFIND", "PROPPATCH", "LOCK", "UNLOCK", "REPORT", "ACL" };

            var allow = new StringValues(allowValues.ToArray());

            Response.Headers.Allow = allow;
            Response.Headers.Add("DAV", "1, 2, 3, access-control, addressbook");

            return Task.FromResult((ActionResult)Ok());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [AcceptVerbs("PROPFIND")]
        public Task<ActionResult<string>> PropfindAsync()
        {
            var xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><note>\r\n<to>Tove</to>\r\n<from>Jani</from>\r\n<heading>Reminder</heading>\r\n<body>Don't forget me this weekend!</body>\r\n</note>";

            return Task.FromResult((ActionResult<string>)Content(xmlString, "text/xml"));
        }
    }
}
