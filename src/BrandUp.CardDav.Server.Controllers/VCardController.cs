using BrandUp.CardDav.Server.Abstractions.Documents;
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
    [Route("principals/{Name}/Collections/{AddressBook}/{Contact}")]
    [Authorize]
    public class VCardController : DavControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="addressBookRepository"></param>
        /// <param name="contactRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VCardController(IUserRepository userRepository, IAddressBookRepository addressBookRepository, IContactRepository contactRepository, ILogger<VCardController> logger)
            : base(userRepository, addressBookRepository, contactRepository, logger)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="addressBook"></param>
        /// <param name="vCardName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAsync([FromRoute(Name = "Name")] string userName,
                                                [FromRoute(Name = "AddressBook")] string addressBook,
                                                [FromRoute(Name = "Contact")] string vCardName)
        {
            try
            {
                var contact = await FindContactAsync(userName, addressBook, vCardName, HttpContext.RequestAborted);
                if (contact == null)
                    return NotFound();
                return Ok(contact.RawVCard);
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="depth"></param>
        /// <param name="responseService"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [CardDavPropfind]
        public async Task<ActionResult> PropfindAsync([FromRoute(Name = "AddressBook")] string bookName, [FromRoute(Name = "Contact")] string contactName, IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            var cancellationToken = HttpContext.RequestAborted;
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (bookName == null)
                return NotFound();
            if (contactName == null)
                return NotFound();

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");
            try
            {
                var userId = User.Identity.GetUserId();
                var addressBook = await addressBookRepository.FindByNameAsync(bookName, userId, cancellationToken);
                if (addressBook == null) return NotFound();

                var contact = await contactRepository.FindByNameAsync(contactName, addressBook.Id, cancellationToken);
                if (contact == null) return NotFound();

                var responseBody = new MultistatusResponseBody();

                var resourse = await ProccessRessposeResourseAsync(request.Handlers, request.Endpoint, contact, request.IsAllProp, cancellationToken);

                responseBody.Resources.Add(resourse);

                Response.StatusCode = 207;
                CustomSerializer.SerializeResponse(Response.Body, responseBody);

                return new EmptyResult();
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="addressBook"></param>
        /// <param name="vCardName"></param>
        /// <param name="Etag"></param>
        /// <returns></returns>
        [HttpPut]
        [Consumes("text/x-vcard", "text/plain", "text/vcard")]
        public async Task<ActionResult> AddOrUpdateAsync([FromRoute(Name = "Name")] string userName,
                                                    [FromRoute(Name = "AddressBook")] string addressBook,
                                                    [FromRoute(Name = "Contact")] string vCardName,
                                                    [FromHeader(Name = "If-Match")] string Etag)
        {
            try
            {
                var contact = await FindContactAsync(userName, addressBook, vCardName, HttpContext.RequestAborted);

                using var reader = new StreamReader(Request.Body);
                var vCard = reader.ReadToEnd();
                if (contact == null)
                {
                    await CreateContactAsync(userName, addressBook, vCardName, vCard, HttpContext.RequestAborted);
                }
                else if (contact.ETag == Etag)
                {
                    contact.RawVCard = vCard;
                    if (!await contactRepository.UpdateAsync(contact, Etag, HttpContext.RequestAborted))
                        return StatusCode(500);
                }
                else return Conflict();

                return NoContent();
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult> DeleteAsync([FromRoute(Name = "AddressBook")] string bookName, [FromRoute(Name = "Contact")] string contactName)
        {
            if (bookName == null || contactName == null)
                return NotFound();

            var cancellationToken = HttpContext.RequestAborted;

            try
            {
                var addressbook = await addressBookRepository.FindByNameAsync(bookName, User.Identity.GetUserId(), cancellationToken);
                if (addressbook == null)
                    return NotFound();

                var contact = await contactRepository.FindByNameAsync(contactName, addressbook.Id, cancellationToken);

                if (contact != null)
                {
                    await contactRepository.DeleteAsync(contact, HttpContext.RequestAborted);
                }
                else return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Helpers 

        async Task<Contact> FindContactAsync(string name, string addressBook, string contactName, CancellationToken cancellationToken)
        {
            var book = await FindAddressBookAsync(name, addressBook, cancellationToken);

            var contact = await contactRepository.FindByNameAsync(contactName, book.Id, cancellationToken);

            return contact;
        }

        async Task<AddressBook> FindAddressBookAsync(string name, string addressBookName, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByNameAsync(name, cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var addresBook = await addressBookRepository.FindByNameAsync(addressBookName, user.Id, cancellationToken);

            if (addresBook == null)
                throw new ArgumentNullException(nameof(user));

            return addresBook;
        }

        async Task CreateContactAsync(string name, string addressBook, string contact, string vcard, CancellationToken cancellationToken)
        {
            var book = await FindAddressBookAsync(name, addressBook, cancellationToken);

            await contactRepository.CreateAsync(contact, book.Id, vcard, cancellationToken);
        }

        #endregion
    }
}
