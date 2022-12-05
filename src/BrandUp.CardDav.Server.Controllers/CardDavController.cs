using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BrandUp.CardDav.Server.Controllers
{
    public abstract class CardDavController : ControllerBase
    {
        protected CardDavController() { }

        protected ActionResult Options()
        {
            var allowValues = new List<string>() { "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "COPY",
                "MOVE", "MKCOL", "PROPFIND", "PROPPATCH", "LOCK", "UNLOCK", "REPORT", "ACL" };

            var allow = new StringValues(allowValues.ToArray());

            Response.Headers.Add("Allow", allow);
            Response.Headers.Add("DAV", "1, 2, 3, access-control, addressbook");

            return Ok();
        }
    }
}
