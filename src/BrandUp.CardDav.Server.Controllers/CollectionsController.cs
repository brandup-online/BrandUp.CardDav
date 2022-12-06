using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("Principal/{Name}/{controller}")]
    public class CollectionsController : CardDavController
    {
        [FromRoute(Name = "Name")]
        private string Name { get; }

        [Consumes("text/xml")]
        [AcceptVerbs("PROPFIND")]
        public Task<ActionResult> PropfindAsync()
        {
            return Task.FromResult((ActionResult)Ok());
        }

        [Consumes("text/xml")]
        [AcceptVerbs("REPORT")]
        public Task<ActionResult> ReportCollectionAsync()
        {
            return Task.FromResult((ActionResult)Ok());
        }

        [Consumes("text/xml")]
        [Route("{AddressBook}")]
        [AcceptVerbs("PROPFIND")]
        public Task<ActionResult> PropfindCollectionAsync([FromRoute] string addressBook)
        {
            return Task.FromResult((ActionResult)Ok());
        }

        [Consumes("text/xml")]
        [AcceptVerbs("REPORT")]
        [Route("{AddressBook}")]
        public Task<ActionResult> ReportCollectionAsync([FromRoute] string addressBook)
        {
            return Task.FromResult((ActionResult)Ok());
        }
    }
}
