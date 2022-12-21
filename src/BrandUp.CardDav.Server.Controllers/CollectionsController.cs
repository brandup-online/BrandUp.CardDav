using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("Principal/{Name}/{controller}")]
    public class CollectionsController : CardDavController
    {
        public CollectionsController(IUserRepository userRepository, IContactRepository contactRepository, IAddressBookRepository addressRepository)
            : base(userRepository, contactRepository, addressRepository)
        {
        }

        #region Propfind controllers

        [CardDavPropfind]
        public async Task<ActionResult<string>> PropfindAsync([FromRoute(Name = "Name")] string name, PropfindRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Depth == Depth.Infinity)
            {
                return BadRequest("Depth: Infinity");
            }

            var user = await userRepository.FindByNameAsync(name, HttpContext.RequestAborted);

            if (user == null)
                return NotFound();

            var response = new PropfindResponseBody();

            response.Resources.Add(GenerateResponseResource(user, request.Body.Properties));

            if (request.Depth.Value == Depth.One.Value)
            {
                var addresBooks = await addressRepository.FindCollectionsByUserIdAsync(user.Id, HttpContext.RequestAborted);
                foreach (var book in addresBooks)
                {
                    response.Resources.Add(GenerateResponseResource(book, request.Body.Properties, true));
                }
            }

            var serializer = new XmlSerializer(typeof(PropfindResponseBody));

            Response.StatusCode = 207;
            serializer.Serialize(Response.Body, response);

            return new EmptyResult();
        }

        [CardDavPropfind("{AddressBook}")]
        public async Task<ActionResult> PropfindCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute] string addressBook, PropfindRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Depth == Depth.Infinity)
                return BadRequest("Depth: Infinity");
            var user = await userRepository.FindByNameAsync(name, HttpContext.RequestAborted);

            if (user == null)
                return NotFound();

            var addresBook = await addressRepository.FindByNameAsync(addressBook, HttpContext.RequestAborted);

            if (addresBook?.UserId != user.Id)
                return NotFound();

            var response = new PropfindResponseBody();

            response.Resources.Add(GenerateResponseResource(addresBook, request.Body.Properties));

            if (request.Depth.Value == Depth.One.Value)
            {
                var contacts = await contactRepository.FindAllContactsByBookIdAsync(addresBook.Id, HttpContext.RequestAborted);
                foreach (var contact in contacts)
                {
                    response.Resources.Add(GenerateResponseResource(contact, request.Body.Properties, true));
                }
            }

            var serializer = new XmlSerializer(typeof(PropfindResponseBody));

            Response.StatusCode = 207;
            serializer.Serialize(Response.Body, response);

            return new EmptyResult();
        }

        #endregion

        #region Report controllers

        [CardDavReport("{AddressBook}")]
        public async Task<ActionResult> ReportCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute] string addressBook, ReportRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await userRepository.FindByNameAsync(name, HttpContext.RequestAborted);

            if (user == null)
                return NotFound();

            var addresBook = await addressRepository.FindByNameAsync(addressBook, HttpContext.RequestAborted);

            if (addresBook?.UserId != user.Id)
                return NotFound();

            var contacts = await contactRepository.FindAllContactsByBookIdAsync(addresBook.Id, HttpContext.RequestAborted);

            var response = new ReportResponseBody();
            foreach (var contact in contacts)
            {
                var defaultResponce = GenerateResponseResource(contact, request.Body.Properties, true);
                response.Resources.Add(new AddressDataResource
                {
                    Endpoint = defaultResponce.Endpoint,
                    FoundProperties = defaultResponce.FoundProperties,
                    NotFoundProperties = defaultResponce.NotFoundProperties,
                });
            }

            var serializer = new XmlSerializer(typeof(ReportResponseBody));

            Response.StatusCode = 207;
            serializer.Serialize(Response.Body, response);

            return new EmptyResult();
        }

        #endregion

        #region Mkcol controllers

        [CardDavMkcol("{AddressBook}")]
        public async Task<ActionResult> MakeCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute] string addressBook)
        {
            var user = await userRepository.FindByNameAsync(name, HttpContext.RequestAborted);

            if (user == null)
                return NotFound();

            var book = await addressRepository.FindByNameAsync(addressBook, HttpContext.RequestAborted);

            if (book != null)
                return Conflict();

            await addressRepository.CreateAsync(addressBook, user.Id, HttpContext.RequestAborted);

            return Created(new Uri(Request.Path.Value, UriKind.Relative), addressBook);
        }

        #endregion
    }
}
