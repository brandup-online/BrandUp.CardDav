using BrandUp.CardDav.Server.Attributes;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Requests.Body.Report;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using BrandUp.CardDav.VCard;
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

        #region Propfind controllers

        [CardDavPropfind]
        public async Task<ActionResult<string>> PropfindAsync([FromRoute(Name = "Name")] string name, PropfindRequest request)
        {
            var endpoint = Request.Path.Value;
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await userRepository.FindByNameAsync(name, HttpContext.RequestAborted);

            if (user == null)
                return NotFound();

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

            Response.StatusCode = 207;
            serializer.Serialize(Response.Body, response);

            return new EmptyResult();
        }

        [CardDavPropfind("{AddressBook}")]
        public Task<ActionResult> PropfindCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute] string addressBook, PropfindRequest request)
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

        #endregion

        #region Report controllers

        [CardDavReport("{AddressBook}")]
        public async Task<ActionResult> ReportCollectionAsync([FromRoute(Name = "Name")] string name, [FromRoute] string addressBook, ReportRequest request)
        {
            var endpoint = Request.Path.Value;
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await userRepository.FindByNameAsync(name, HttpContext.RequestAborted);

            if (user == null)
                return NotFound();

            var response = new ReportResponseBody();
            Dictionary<IDavProperty, string> propertyDictionary = new();
            foreach (var property in request.Body.Properties)
            {
                if (property.Name == "getctag")
                    propertyDictionary.Add(property, user.CTag);
            }

            var addressBooks = await addressRepository.FindCollectionsByUserIdAsync(user.Id, HttpContext.RequestAborted);
            if (!addressBooks.Select(b => b.UserId).Contains(user.Id))
                return NotFound();

            var book = addressBooks.First(b => b.Name == addressBook);

            var contacts = await contactRepository.FindAllContactsByBookIdAsync(book.Id, HttpContext.RequestAborted);

            var dict = contacts.ToDictionary(k => k, v => VCardParser.Parse(v.RawVCard));

            if (request.Body is AddresbookQueryBody body)
            {

            }
            else if (request.Body is MultigetBody multigetBody)
            {
                var ids = multigetBody.VCardEndpoints.Select(s => Guid.Parse(s.Split("/").Last()));
                contacts = dict.Keys.Where(k => ids.Contains(k.Id)).ToList();
            }

            foreach (var contact in contacts)
            {
                propertyDictionary = new();
                foreach (var property in request.Body.Properties)
                {
                    if (property.Name == "getetag")
                        propertyDictionary.Add(property, contact.ETag);
                    if (property is AddressData addressData)
                    {
                        var vCard = dict[contact];
                        if (addressData.VCardProperies.Any())
                            propertyDictionary.Add(property, vCard.ToStringProps(addressData.VCardProperies));
                        else
                            propertyDictionary.Add(property, vCard.ToString());
                    }
                }

                response.Resources.Add(new AddressDataResource
                {
                    Endpoint = endpoint,
                    FoundProperties = new(propertyDictionary),
                    NotFoundProperties = request.Body.Properties.Except(propertyDictionary.Keys, new PropertyComparer()).ToArray()
                });
            }

            var serializer = new XmlSerializer(typeof(PropfindResponseBody));

            Response.StatusCode = 207;
            serializer.Serialize(Response.Body, response);

            return new EmptyResult();
        }

        #endregion

        #region Mkcol controllers

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
                            " <D:href>http://www.example.com/papers/</D:href>\r\n   " +
                            "  <D:propstat>\r\n   " +
                            "      <D:prop>\r\n        " +
                            "     </D:prop>\r\n      " +
                            "   <D:status>HTTP/1.1 200 OK</D:status>\r\n     " +
                            "  </D:propstat>\r\n   " +
                            "  </D:response>\r\n " +
                            "  </D:multistatus>";

            return Task.FromResult((ActionResult)Content(xmlString, "text/xml"));
        }

        #endregion
    }
}
