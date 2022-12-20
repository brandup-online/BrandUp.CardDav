using BrandUp.CardDav.Server.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : CardDavController
    {
        readonly IEnumerable<EndpointDataSource> endpointSources;

        public HomeController(IEnumerable<EndpointDataSource> endpointSources, IUserRepository userRepository, IContactRepository contactRepository, IAddressBookRepository addressRepository)
            : base(userRepository, contactRepository, addressRepository)
        {
            this.endpointSources = endpointSources;
        }

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
