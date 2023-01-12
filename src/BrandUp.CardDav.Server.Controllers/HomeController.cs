using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Abstractions.Exceptions;
using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Transport.Binding;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<ActionResult> GetAsync()
        {
            return Task.FromResult((ActionResult)BadRequest());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public Task<ActionResult> OptionsAsync()
        {
            var allowValues = new List<string>() { "OPTIONS", "GET", "POST", "PUT", "DELETE", "MKCOL", "PROPFIND", "REPORT" };

            var allow = new StringValues(allowValues.ToArray());

            Response.Headers.Add("Allow", allow);
            Response.Headers.Add("DAV", "1, addressbook");

            return Task.FromResult((ActionResult)Ok());
        }

        [CardDavPropfind(".well-known/carddav")]
        public ActionResult WellKnown()
        {
            return RedirectToAction("Propfind", "Collections", new { Name = User.Identity.Name });
        }

        [CardDavPropfind("principals")]
        public ActionResult PrincipalsAsync(IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            logger.LogInformation($"Incoming request: Depth:{depth}");

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            try
            {
                var dictionary = new Dictionary<string, IDavDocument> { { string.Join("/", request.Endpoint, request.Document.Name, "Collections"), request.Document } };

                var response = request.Body.CreateResponse(dictionary);

                var serializer = new XmlSerializer(typeof(PropfindResponseBody));

                Response.StatusCode = 207;
                serializer.Serialize(Response.Body, response);

                return new EmptyResult();
            }
            catch (ArgumentNullException)
            {
                logger.LogError("Not found");

                return NotFound();
            }
            catch (DavPropertyException ex)
            {
                logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
        }
    }
}
