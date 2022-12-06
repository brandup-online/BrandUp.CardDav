using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("Principal/{Name}/Collections/{AddressBook}/{VCard}")]
    public class VCardController : CardDavController
    {
        [HttpGet]
        public Task<ActionResult> GetAsync([FromRoute(Name = "VCard")] string vCardName)
        {
            return Task.FromResult((ActionResult)Ok());
        }

        [HttpPut]
        public Task<ActionResult> AddOrUpdateAsync([FromRoute(Name = "VCard")] string vCardName, [FromHeader(Name = "If-Match")] string Etag)
        {
            return Task.FromResult((ActionResult)NoContent());
        }

        [HttpDelete]
        public Task<ActionResult> DeleteAsync([FromRoute(Name = "VCard")] string vCardName)
        {
            return Task.FromResult((ActionResult)NoContent());
        }
    }
}
