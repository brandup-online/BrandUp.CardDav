using BrandUp.CardDav.Server.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : CardDavController
    {
        public HomeController(IUserRepository userRepository, IContactRepository contactRepository, IAddressBookRepository addressRepository)
            : base(userRepository, contactRepository, addressRepository)
        {
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
