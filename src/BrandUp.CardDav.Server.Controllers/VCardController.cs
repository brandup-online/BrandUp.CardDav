using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Services;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("Principal/{Name}/Collections/{AddressBook}/{VCard}")]
    public class VCardController : CardDavController
    {
        public VCardController(IResponseService responseService)
            : base(responseService)
        {
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync([FromRoute(Name = "Name")] string userName,
                                                [FromRoute(Name = "AddressBook")] string addressBook,
                                                [FromRoute(Name = "VCard")] string vCardName)
        {
            try
            {
                var contact = await responseService.FindContactAsync(userName, addressBook, vCardName, HttpContext.RequestAborted);
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

        [CardDavPropfind]
        public async Task<ActionResult> PropfindAsync([FromRoute(Name = "Name")] string name,
            [FromRoute(Name = "AddressBook")] string addressBookName,
            [FromRoute(Name = "VCard")] string contact,
            PropfindRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Depth == Depth.Infinity)
                return BadRequest("Depth: Infinity");

            if (request.Depth == Depth.One)
                return BadRequest("Depth: One");

            try
            {
                var contactDocument = await responseService.FindContactAsync(name, addressBookName, contact, HttpContext.RequestAborted);

                var response = await responseService.ProcessPropfindAsync(contactDocument, request, HttpContext.RequestAborted);

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


        [HttpPut]
        [Consumes("text/x-vcard", "text/plain", "text/vcard")]
        public async Task<ActionResult> AddOrUpdateAsync([FromRoute(Name = "Name")] string userName,
                                                    [FromRoute(Name = "AddressBook")] string addressBook,
                                                    [FromRoute(Name = "VCard")] string vCardName,
                                                    [FromHeader(Name = "If-Match")] string Etag)
        {
            try
            {
                var contact = await responseService.FindContactAsync(userName, addressBook, vCardName, HttpContext.RequestAborted);

                using var reader = new StreamReader(Request.Body);
                var vCard = reader.ReadToEnd();
                if (contact == null)
                {
                    await responseService.CreateContactAsync(userName, addressBook, vCardName, vCard, HttpContext.RequestAborted);
                }
                else if (contact.ETag == Etag)
                {
                    contact.RawVCard = vCard;
                    if (!await responseService.UpdateContactAsync(contact, Etag, HttpContext.RequestAborted))
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

        [HttpDelete]
        public async Task<ActionResult> DeleteAsync([FromRoute(Name = "Name")] string userName,
                                             [FromRoute(Name = "AddressBook")] string addressBook,
                                             [FromRoute(Name = "VCard")] string vCardName)
        {
            try
            {
                var contact = await responseService.FindContactAsync(userName, addressBook, vCardName, HttpContext.RequestAborted);

                if (contact != null)
                {
                    await responseService.DeleteContactAsync(contact, HttpContext.RequestAborted);
                }
                else return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
