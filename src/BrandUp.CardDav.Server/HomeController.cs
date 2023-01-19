using BrandUp.CardDav.Server.Abstractions.Exceptions;
using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using BrandUp.CardDav.Transport.Server.Binding;
using BrandUp.CardDav.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace BrandUp.CardDav.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("")]
    [Authorize]
    [ServerAuthorize]
    public class HomeController : DavControllerBase
    {
        public HomeController(IUserRepository userRepository, IAddressBookRepository adddressBookRepository, IContactRepository contactRepository, ILogger<HomeController> logger)
            : base(userRepository, adddressBookRepository, contactRepository, logger)
        { }

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
        public ActionResult WellKnown(IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            return Redirect($"principals/{User.Identity.Name}/Collections");
        }

        [CardDavPropfind("principals")]
        public async Task<ActionResult> Principals(IncomingRequest request, [FromHeader(Name = "Depth")] string depth)
        {
            var cancellationToken = HttpContext.RequestAborted;

            logger.LogInformation($"Incoming request: Depth:{depth}");

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (depth == Depth.Infinity.Value)
                return BadRequest("Depth: Infinity");

            try
            {
                var responseBody = new MultistatusResponseBody();

                var resourse = await ProccessRessposeResourseAsync(request.Handlers, request.Endpoint, null, request.IsAllProp, cancellationToken);

                responseBody.Resources.Add(resourse);

                if (depth == Depth.One.Value)
                {
                    var user = await userRepository.FindByNameAsync(HttpContext.User.Identity.Name, cancellationToken);

                    var userEndpoint = string.Join("/", request.Endpoint, user.Name, "Collections");

                    var userResourse = await ProccessRessposeResourseAsync(request.Handlers, userEndpoint, null, request.IsAllProp, cancellationToken);

                    responseBody.Resources.Add(userResourse);
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
