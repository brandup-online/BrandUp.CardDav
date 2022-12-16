using BrandUp.CardDav.Server.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace BrandUp.CardDav.Server.Controllers
{
    public abstract class CardDavController : ControllerBase
    {
        readonly protected IUserRepository userRepository;
        readonly protected IContactRepository contactRepository;
        readonly protected IAddressBookRepository addressRepository;
        protected CardDavController(IUserRepository userRepository, IContactRepository contactRepository, IAddressBookRepository addressRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            this.addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
        }

        protected ActionResult Options()
        {
            var allowValues = new List<string>() { "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "MKCOL", "PROPFIND", "REPORT" };

            var allow = new StringValues(allowValues.ToArray());

            Response.Headers.Add("Allow", allow);
            Response.Headers.Add("DAV", "1, 3, access-control, addressbook");

            return Ok();
        }
    }
}
