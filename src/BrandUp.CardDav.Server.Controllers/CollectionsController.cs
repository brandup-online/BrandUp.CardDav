using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Models.Abstract;
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

        [FromRoute(Name = "Name")]
        private string Name { get; }

        [CardDavPropfind]
        public async Task<ActionResult<string>> PropfindAsync(PropfindRequest request)
        {
            var endpoint = ControllerContext.ActionDescriptor.AttributeRouteInfo.Name;
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await userRepository.FindByNameAsync(Name, HttpContext.RequestAborted);

            var response = new PropfindResponseBody();
            Dictionary<IDavProperty, string> propertyDictionary = new();
            foreach (var property in request.Body.Properties)
            {
                if (property.Name == "getctag")
                    propertyDictionary.Add(property, user.CTag);
            }

            response.Resources.Add(new DefaultResponseResource
            {
                Endpoint = endpoint,
                FoundProperties = new(propertyDictionary),
                NotFoundProperties = request.Body.Properties.Except(propertyDictionary.Keys, new PropertyComparer()).ToArray()
            });

            if (request.Depth == Depth.One)
            {
                var addresBooks = await addressRepository.FindCollectionsByUserIdAsync(user.Id, HttpContext.RequestAborted);
                foreach (var book in addresBooks)
                {
                    propertyDictionary = new();
                    foreach (var property in request.Body.Properties)
                    {
                        if (property.Name == "getctag")
                            propertyDictionary.Add(property, book.CTag);
                    }

                    response.Resources.Add(new DefaultResponseResource
                    {
                        Endpoint = endpoint,
                        FoundProperties = new(propertyDictionary),
                        NotFoundProperties = request.Body.Properties.Except(propertyDictionary.Keys, new PropertyComparer()).ToArray()
                    });
                }
            }
            else if (request.Depth == Depth.Infinity)
            {
                return BadRequest();
            }

            var serializer = new XmlSerializer(typeof(PropfindResponseBody));

            serializer.Serialize(Response.Body, response);

            return StatusCode(207);
        }

        [CardDavReport]
        public Task<ActionResult> ReportCollectionAsync(ReportRequest request)
        {
            var xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n  " +
                            " <D:multistatus xmlns:D=\"DAV:\">\r\n   " +
                            "  <D:response>\r\n      " +
                            " <D:href>http://www.example.com/papers/</D:href>\r\n     " +
                            "  <D:propstat>\r\n   " +
                            "      <D:prop>\r\n        " +
                            "     </D:prop>\r\n      " +
                            "   <D:status>HTTP/1.1 200 OK</D:status>\r\n     " +
                            "  </D:propstat>\r\n   " +
                            "  </D:response>\r\n " +
                            "  </D:multistatus>";

            return Task.FromResult((ActionResult)Content(xmlString, "text/xml"));
        }

        [CardDavMkcol]
        public Task<ActionResult> MakeCollectionAsync(MkcolRequest request)
        {
            var xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n  " +
                            " <D:multistatus xmlns:D=\"DAV:\">\r\n   " +
                            "  <D:response>\r\n      " +
                            " <D:href>http://www.example.com/papers/</D:href>\r\n     " +
                            "  <D:propstat>\r\n   " +
                            "      <D:prop>\r\n        " +
                            "     </D:prop>\r\n      " +
                            "   <D:status>HTTP/1.1 200 OK</D:status>\r\n     " +
                            "  </D:propstat>\r\n   " +
                            "  </D:response>\r\n " +
                            "  </D:multistatus>";

            return Task.FromResult((ActionResult)Content(xmlString, "text/xml"));
        }

        [CardDavMkcol("{AddressBook}")]
        public Task<ActionResult> MakeCollectionAsync([FromRoute] string addressBook)
        {
            var xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n  " +
                            " <D:multistatus xmlns:D=\"DAV:\">\r\n   " +
                            "  <D:response>\r\n      " +
                            " <D:href>http://www.example.com/papers/</D:href>\r\n     " +
                            "  <D:propstat>\r\n   " +
                            "      <D:prop>\r\n        " +
                            "     </D:prop>\r\n      " +
                            "   <D:status>HTTP/1.1 200 OK</D:status>\r\n     " +
                            "  </D:propstat>\r\n   " +
                            "  </D:response>\r\n " +
                            "  </D:multistatus>";

            return Task.FromResult((ActionResult)Content(xmlString, "text/xml"));
        }


        [CardDavPropfind("{AddressBook}")]
        public Task<ActionResult> PropfindCollectionAsync([FromRoute] string addressBook, PropfindRequest request)
        {
            var xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n  " +
                            " <D:multistatus xmlns:D=\"DAV:\">\r\n   " +
                            "  <D:response>\r\n      " +
                            " <D:href>http://www.example.com/papers/</D:href>\r\n     " +
                            "  <D:propstat>\r\n   " +
                            "      <D:prop>\r\n        " +
                            "     </D:prop>\r\n      " +
                            "   <D:status>HTTP/1.1 200 OK</D:status>\r\n     " +
                            "  </D:propstat>\r\n   " +
                            "  </D:response>\r\n " +
                            "  </D:multistatus>";

            return Task.FromResult((ActionResult)Content(xmlString, "text/xml"));
        }

        [CardDavReport("{AddressBook}")]
        public Task<ActionResult> ReportCollectionAsync([FromRoute] string addressBook, ReportRequest request)
        {
            var xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n  " +
                        " <D:multistatus xmlns:D=\"DAV:\">\r\n   " +
                        "  <D:response>\r\n      " +
                        " <D:href>http://www.example.com/papers/</D:href>\r\n     " +
                        "  <D:propstat>\r\n   " +
                        "      <D:prop>\r\n        " +
                        "     </D:prop>\r\n      " +
                        "   <D:status>HTTP/1.1 200 OK</D:status>\r\n     " +
                        "  </D:propstat>\r\n   " +
                        "  </D:response>\r\n " +
                        "  </D:multistatus>";

            return Task.FromResult((ActionResult)Content(xmlString, "text/xml"));
        }
    }
}
