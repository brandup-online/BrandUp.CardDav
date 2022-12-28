using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Services;
using BrandUp.CardDav.Services.Exceptions;
using BrandUp.CardDav.Transport.Binding;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Server.Controllers
{
    [ApiController]
    [Route("Principal/{Name}/{controller}")]
    public class CollectionsController : ControllerBase
    {
        readonly IResponseService responseService;

        public CollectionsController(IResponseService responseService)
        {
            this.responseService = responseService ?? throw new ArgumentNullException(nameof(responseService));
        }

        #region Propfind controllers

        [CardDavPropfind]
        public async Task<ActionResult<string>> PropfindAsync(IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            try
            {
                var response = await responseService.ProcessPropfindAsync(request, depth, HttpContext.RequestAborted);

                var serializer = new XmlSerializer(typeof(PropfindResponseBody));

                Response.StatusCode = 207;
                serializer.Serialize(Response.Body, response);

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

        [CardDavPropfind("{AddressBook}")]
        public async Task<ActionResult> PropfindCollectionAsync(IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            try
            {
                var response = await responseService.ProcessPropfindAsync(request, depth, HttpContext.RequestAborted);

                var serializer = new XmlSerializer(typeof(PropfindResponseBody));

                Response.StatusCode = 207;
                serializer.Serialize(Response.Body, response);

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

        [CardDavReport("{AddressBook}")]
        public async Task<ActionResult> ReportCollectionAsync(IncomingRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var response = await responseService.ProcessReportAsync(request, HttpContext.RequestAborted);

                var serializer = new XmlSerializer(typeof(ReportResponseBody));

                Response.StatusCode = 207;
                serializer.Serialize(Response.Body, response);

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

        [CardDavMkcol("{AddressBook}")]
        public async Task<ActionResult> MakeCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute(Name = "AddressBook")] string addressBook)
        {
            try
            {
                if (await responseService.MakeCollectionAsync(name, addressBook, HttpContext.RequestAborted))
                    return Created(new Uri(Request.Path.Value, UriKind.Relative), addressBook);

                else return BadRequest();
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (ConflictException)
            {
                return Conflict();
            }
        }

        #endregion
    }
}
