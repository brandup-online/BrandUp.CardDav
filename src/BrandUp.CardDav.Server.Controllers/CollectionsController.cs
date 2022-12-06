using BrandUp.CardDav.Server.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("Principal/{Name}/{controller}")]
    public class CollectionsController : CardDavController
    {
        [FromRoute(Name = "Name")]
        private string Name { get; }

        [CardDavPropfind]
        public Task<ActionResult> PropfindAsync()
        {
            return Task.FromResult((ActionResult)Ok());
        }

        [CardDavReport]
        public Task<ActionResult> ReportCollectionAsync()
        {
            return Task.FromResult((ActionResult)Ok());
        }


        [CardDavPropfind("{AddressBook}")]
        public Task<ActionResult> PropfindCollectionAsync([FromRoute] string addressBook)
        {
            return Task.FromResult((ActionResult)Ok());
        }

        [CardDavReport("{AddressBook}")]
        public Task<ActionResult> ReportCollectionAsync([FromRoute] string addressBook)
        {
            return Task.FromResult((ActionResult)Ok());
        }
    }
}
