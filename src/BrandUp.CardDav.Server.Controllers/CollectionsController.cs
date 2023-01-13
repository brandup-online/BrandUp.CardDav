using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Abstractions.Exceptions;
using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Binding;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Xml;
using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("principals/{Name}/{controller}")]
    public class CollectionsController : ControllerBase
    {
        readonly IUserRepository userRepository;
        readonly IAddressBookRepository addressBookRepository;
        readonly IContactRepository contactRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="addressBookRepository"></param>
        /// <param name="contactRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CollectionsController(IUserRepository userRepository, IAddressBookRepository addressBookRepository, IContactRepository contactRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.addressBookRepository = addressBookRepository ?? throw new ArgumentNullException(nameof(addressBookRepository));
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
        }
        #region Propfind controllers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CardDavPropfind]
        public async Task<ActionResult<string>> PropfindAsync(IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            try
            {
                var dictionary = new Dictionary<string, IDavDocument> { { request.Endpoint, request.Document } };
                if (depth == Depth.One.Value)
                {
                    var documents = await addressBookRepository.FindCollectionsByUserIdAsync(request.Document.Id, HttpContext.RequestAborted);
                    foreach (var document in documents)
                    {
                        dictionary.Add(string.Join('/', request.Endpoint, document.Name), document);
                    }
                }

                var response = request.Body.CreateResponse(dictionary);

                Response.StatusCode = 207;
                CustomSerializer.SerializeResponse(Response.Body, response);

                return new EmptyResult();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (DavPropertyException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CardDavPropfind("{AddressBook}")]
        public async Task<ActionResult> PropfindCollectionAsync(IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            try
            {
                var dictionary = new Dictionary<string, IDavDocument> { { request.Endpoint, request.Document } };
                if (depth == Depth.One.Value)
                {
                    var documents = await contactRepository.FindAllContactsByBookIdAsync(request.Document.Id, HttpContext.RequestAborted);
                    foreach (var document in documents)
                    {
                        dictionary.Add(string.Join('/', request.Endpoint, document.Name), document);
                    }
                }

                var response = request.Body.CreateResponse(dictionary);

                Response.StatusCode = 207;
                CustomSerializer.SerializeResponse(Response.Body, response);

                return new EmptyResult();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (DavPropertyException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Report controllers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CardDavReport("{AddressBook}")]
        public async Task<ActionResult> ReportCollectionAsync(IncomingRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var dictionary = new Dictionary<string, IDavDocument>();

                var documents = await contactRepository.FindAllContactsByBookIdAsync(request.Document.Id, HttpContext.RequestAborted);
                foreach (var document in documents)
                {
                    dictionary.Add(string.Join('/', request.Endpoint, document.Name), document);
                }

                var response = request.Body.CreateResponse(dictionary);

                Response.StatusCode = 207;
                CustomSerializer.SerializeResponse(Response.Body, response);

                return new EmptyResult();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (DavPropertyException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        #endregion

        #region Mkcol controllers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="addressBook"></param>
        /// <returns></returns>
        [CardDavMkcol("{AddressBook}")]
        public async Task<ActionResult> MakeCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute(Name = "AddressBook")] string addressBook)
        {
            try
            {
                var user = await userRepository.FindByNameAsync(name, HttpContext.RequestAborted);

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                var book = await addressBookRepository.FindByNameAsync(addressBook, user.Id, HttpContext.RequestAborted);

                if (book != null)
                    throw new ConflictException(book, addressBook);

                await addressBookRepository.CreateAsync(addressBook, user.Id, HttpContext.RequestAborted);

                return Created(new Uri(Request.Path.Value, UriKind.Relative), addressBook);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (ConflictException)
            {
                return Conflict();
            }
            catch
            {
                return BadRequest();
            }
        }

        #endregion
    }
}
