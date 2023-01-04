using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Services;
using BrandUp.CardDav.Transport.Binding;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("Principal/{Name}/Collections/{AddressBook}/{Contact}")]
    public class VCardController : ControllerBase
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
        public VCardController(IUserRepository userRepository, IAddressBookRepository addressBookRepository, IContactRepository contactRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.addressBookRepository = addressBookRepository ?? throw new ArgumentNullException(nameof(addressBookRepository));
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
        }

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
        public async Task<ActionResult> PropfindAsync(IncomingRequest request, [FromHeader(Name = "Depth")] string depth, [FromServices] IResponseService responseService)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            if (depth == Depth.One.Value)
                return BadRequest("Depth: One");

            try
            {
                var response = await responseService.ProcessPropfindAsync(request, depth,
                    HttpContext.RequestAborted);

                var serializer = new XmlSerializer(typeof(PropfindResponseBody));

                Response.StatusCode = 207;
                serializer.Serialize(Response.Body, response);

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
        public async Task<ActionResult> DeleteAsync(IncomingRequest request)
        {
            try
            {
                if (request.Document is IContactDocument contactDocument)
                {
                    if (contactDocument != null)
                    {
                        await contactRepository.DeleteAsync(contactDocument, HttpContext.RequestAborted);
                    }
                    else return NotFound();
                }
                else
                {
                    return BadRequest();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Helpers 

        async Task<IContactDocument> FindContactAsync(string name, string addressBook, string contactName, CancellationToken cancellationToken)
        {
            var book = await FindAddressBookAsync(name, addressBook, cancellationToken);

            var contact = await contactRepository.FindByNameAsync(contactName, book.Id, cancellationToken);

            return contact;
        }

        async Task<IAddressBookDocument> FindAddressBookAsync(string name, string addressBookName, CancellationToken cancellationToken)
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
