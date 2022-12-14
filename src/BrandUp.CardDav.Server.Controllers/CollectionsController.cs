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

            return Task.FromResult((ActionResult)Content(xmlString, "text/xml"));
        }

        [CardDavReport]
        public Task<ActionResult> ReportCollectionAsync()
        {
            return Task.FromResult((ActionResult)Ok());
        }


        [CardDavPropfind("{AddressBook}")]
        public Task<ActionResult> PropfindCollectionAsync([FromRoute] string addressBook)
        {
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

            return Task.FromResult((ActionResult)Content(xmlString, "text/xml"));
        }

        [CardDavReport("{AddressBook}")]
        public Task<ActionResult> ReportCollectionAsync([FromRoute] string addressBook)
        {
            return Task.FromResult((ActionResult)Ok());
        }
    }
}
