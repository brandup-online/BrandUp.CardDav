using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : CardDavController
    {
        [HttpGet]
        public Task<ActionResult> GetAsync()
        {
            return Task.FromResult((ActionResult)BadRequest());
        }

        [HttpOptions]
        public Task<ActionResult> OptionsAsync()
        {
            return Task.FromResult(Options());
        }


    }
}
