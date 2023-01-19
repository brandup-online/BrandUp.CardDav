using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Abstractions.Exceptions;
using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Server.Extentions;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using BrandUp.CardDav.Transport.Server.Binding;
using BrandUp.CardDav.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrandUp.CardDav.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("principals/{Name}/{controller}")]
    [Authorize]
    [ServerAuthorize]
    public class CollectionsController : DavControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="addressBookRepository"></param>
        /// <param name="contactRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CollectionsController(IUserRepository userRepository, IAddressBookRepository addressBookRepository, IContactRepository contactRepository, ILogger<CollectionsController> logger)
            : base(userRepository, addressBookRepository, contactRepository, logger)
        { }

        #region Propfind controllers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CardDavPropfind]
        public async Task<ActionResult<string>> PropfindAsync([FromRoute(Name = "Name")] string name, IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var cancellationToken = HttpContext.RequestAborted;

            logger.LogWarning($"Incoming request: Depth:{depth}");

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            try
            {
                var userId = User.Identity.GetUserId();
                var user = await userRepository.FindByIdAsync(userId, cancellationToken);

                var responseBody = new MultistatusResponseBody();

                var resourse = await ProccessRessposeResourseAsync(request.Handlers, request.Endpoint, user, request.IsAllProp, cancellationToken);

                responseBody.Resources.Add(resourse);

                if (depth == Depth.One.Value)
                {
                    var addressbooks = await addressBookRepository.FindCollectionsByUserIdAsync(userId, cancellationToken);
                    foreach (var book in addressbooks)
                    {
                        var endpoint = string.Join('/', request.Endpoint, book.Name);
                        resourse = await ProccessRessposeResourseAsync(request.Handlers, endpoint, book, request.IsAllProp, cancellationToken);

                        responseBody.Resources.Add(resourse);
                    }
                }

                Response.StatusCode = 207;

                #region For tests. Delete later.

                using var ms = new MemoryStream();

                CustomSerializer.SerializeResponse(ms, responseBody);

                using var reader = new StreamReader(ms);

                logger.LogWarning(await reader.ReadToEndAsync(cancellationToken));

                ms.Position = 0;

                await ms.CopyToAsync(Response.Body, cancellationToken);

                #endregion

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
        public async Task<ActionResult> PropfindCollectionAsync([FromRoute(Name = "AddressBook")] string name, IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            var cancellationToken = HttpContext.RequestAborted;

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            try
            {
                var userId = User.Identity.GetUserId();
                var addressBook = await addressBookRepository.FindByNameAsync(name, userId, cancellationToken);

                var responseBody = new MultistatusResponseBody();

                var resourse = await ProccessRessposeResourseAsync(request.Handlers, request.Endpoint, addressBook, request.IsAllProp, cancellationToken);

                responseBody.Resources.Add(resourse);

                if (depth == Depth.One.Value)
                {
                    var contacts = await contactRepository.FindAllContactsByBookIdAsync(addressBook.Id, cancellationToken);
                    foreach (var contact in contacts)
                    {
                        var endpoint = string.Join('/', request.Endpoint, contact.Name);
                        resourse = await ProccessRessposeResourseAsync(request.Handlers, endpoint, contact, request.IsAllProp, cancellationToken);

                        responseBody.Resources.Add(resourse);
                    }
                }
                Response.StatusCode = 207;

                #region For tests. Delete later.

                using var ms = new MemoryStream();

                CustomSerializer.SerializeResponse(ms, responseBody);

                using var reader = new StreamReader(ms);

                logger.LogWarning(await reader.ReadToEndAsync(cancellationToken));

                ms.Position = 0;

                await ms.CopyToAsync(Response.Body, cancellationToken);

                #endregion

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
        public async Task<ActionResult> ReportCollectionAsync([FromRoute(Name = "AddressBook")] string name, IncomingRequest request)
        {
            var cancellationToken = HttpContext.RequestAborted;

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var userId = User.Identity.GetUserId();
                var addressBook = await addressBookRepository.FindByNameAsync(name, userId, cancellationToken);

                var responseBody = new MultistatusResponseBody();

                var contacts = await contactRepository.FindAllContactsByBookIdAsync(addressBook.Id, cancellationToken);

                if (request.Filter != null)
                    contacts = request.Filter.FilterCollection(contacts).Cast<Contact>();

                foreach (var contact in contacts)
                {
                    var endpoint = string.Join('/', request.Endpoint, contact.Name);
                    var resourse = await ProccessRessposeResourseAsync(request.Handlers, request.Endpoint, contact, request.IsAllProp, cancellationToken);

                    responseBody.Resources.Add(resourse);
                }

                Response.StatusCode = 207;


                #region For tests. Delete later.

                using var ms = new MemoryStream();

                CustomSerializer.SerializeResponse(ms, responseBody);

                using var reader = new StreamReader(ms);

                logger.LogWarning(await reader.ReadToEndAsync(cancellationToken));

                ms.Position = 0;

                await ms.CopyToAsync(Response.Body, cancellationToken);

                #endregion

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
        public async Task<ActionResult> MakeCollectionAsync([FromRoute(Name = "AddressBook")] string addressBook)
        {
            try
            {
                var user = await userRepository.FindByNameAsync(User.Identity.Name, HttpContext.RequestAborted);

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
