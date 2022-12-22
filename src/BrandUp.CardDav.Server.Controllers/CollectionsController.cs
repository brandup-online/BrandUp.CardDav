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
    [Route("Principal/{Name}/{controller}")]
    public class CollectionsController : CardDavController
    {
        public CollectionsController(IResponseService responseService)
            : base(responseService)
        {
        }

        #region Propfind controllers

        [CardDavPropfind]
        public async Task<ActionResult<string>> PropfindAsync([FromRoute(Name = "Name")] string name, PropfindRequest request)
        {

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Depth == Depth.Infinity)
                return BadRequest("Depth: Infinity");

            try
            {
                var user = await responseService.FindUserAsync(name, HttpContext.RequestAborted);

                var response = await responseService.ProcessPropfindAsync(user, request, HttpContext.RequestAborted);

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

        [CardDavPropfind("{AddressBook}")]
        public async Task<ActionResult> PropfindCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute(Name = "AddressBook")] string addressBookName, PropfindRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Depth == Depth.Infinity)
                return BadRequest("Depth: Infinity");

            try
            {
                var addressBook = await responseService.FindAddressBookAsync(name, addressBookName, HttpContext.RequestAborted);

                var response = await responseService.ProcessPropfindAsync(addressBook, request, HttpContext.RequestAborted);

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

        #endregion

        #region Report controllers

        [CardDavReport("{AddressBook}")]
        public async Task<ActionResult> ReportCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute(Name = "AddressBook")] string addressBookName, ReportRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var addressBook = await responseService.FindAddressBookAsync(name, addressBookName, HttpContext.RequestAborted);

                var response = await responseService.ProcessReportAsync(addressBook, request, HttpContext.RequestAborted);

                var serializer = new XmlSerializer(typeof(ReportResponseBody));

                Response.StatusCode = 207;
                serializer.Serialize(Response.Body, response);

                return new EmptyResult();
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
        }

        #endregion

        #region Mkcol controllers

        [CardDavMkcol("{AddressBook}")]
        public async Task<ActionResult> MakeCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute(Name = "AddressBook")] string addressBook)
        {
            try
            {
                if (await responseService.MakeCollectionAsync(name, addressBook, HttpContext.RequestAborted))
                    return Created(new Uri(Request.Path.Value, UriKind.Relative), addressBook);

                else return BadRequest();
            }
            catch (ArgumentNullException ex)
            {
                return NotFound(ex);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex);
            }
        }

        #endregion
    }
}
