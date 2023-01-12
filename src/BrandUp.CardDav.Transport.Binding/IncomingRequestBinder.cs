using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Binding.Exceptions;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Requests.Body.Propfind;
using BrandUp.CardDav.Transport.Models.Requests.Body.Report;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Binding
{
    public class IncomingRequestBinder : IModelBinder
    {
        readonly IUserRepository userRepository;
        readonly IAddressBookRepository addressBookRepository;
        readonly IContactRepository contactRepository;
        readonly ILogger<IncomingRequestBinder> logger;

        public IncomingRequestBinder(IUserRepository userRepository, IAddressBookRepository addressBookRepository, IContactRepository contactRepository, ILogger<IncomingRequestBinder> logger)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.addressBookRepository = addressBookRepository ?? throw new ArgumentNullException(nameof(addressBookRepository));
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IModelBinder member

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                logger.LogInformation($"Binding incoming request:");

                logger.LogInformation($"User: {bindingContext.HttpContext.User.Identity.Name}");

                var method = bindingContext.HttpContext.Request.Method;

                logger.LogInformation($"Method: {method}");

                IResponseCreator body = null;
                if (method == "PROPFIND")
                {
                    body = GetPropfindRequest(bindingContext);
                }
                else if (method == "REPORT")
                {
                    body = GetReportRequest(bindingContext);
                }

                var incomingRequest = new IncomingRequest()
                {
                    Body = body,
                    Document = await GetDocumentAsync(bindingContext),
                    Endpoint = bindingContext.HttpContext.Request.Path
                };

                bindingContext.Result = ModelBindingResult.Success(incomingRequest);
            }
            catch (BindingException ex)
            {
                logger.LogError(ex.Message);

                bindingContext.ModelState.AddModelError("Path", ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogError(ex.Message);

                bindingContext.ModelState.AddModelError("Document", ex.Message);
            }
            catch (XmlDeserializeException ex)
            {
                logger.LogError(ex.Message);

                bindingContext.ModelState.AddModelError("Body", ex.Message);
            }
        }

        #endregion

        #region Helpers

        private async Task<IDavDocument> GetDocumentAsync(ModelBindingContext bindingContext)
        {
            var values = bindingContext.HttpContext.GetRouteData().Values;

            if (values.TryGetValue("Name", out var name))
            {
                var userDocument = await userRepository.FindByNameAsync((string)name, bindingContext.HttpContext.RequestAborted);
                if (userDocument == null)
                    throw new ArgumentNullException(nameof(userDocument));


                if (!values.TryGetValue("AddressBook", out var addressBook))
                {
                    return userDocument;
                }
                else
                {
                    var bookDocument = await addressBookRepository.FindByNameAsync((string)addressBook, userDocument.Id, bindingContext.HttpContext.RequestAborted);
                    if (bookDocument == null)
                        throw new ArgumentNullException(nameof(bookDocument));

                    if (!values.TryGetValue("Contact", out var contact))
                    {
                        return bookDocument;
                    }
                    else
                    {
                        var contactDocument = await contactRepository.FindByNameAsync((string)contact, bookDocument.Id, bindingContext.HttpContext.RequestAborted);
                        if (contactDocument == null)
                            throw new ArgumentNullException(nameof(contactDocument));

                        return contactDocument;
                    }
                }
            }
            else
            {
                var userDocument = await userRepository.FindByNameAsync(bindingContext.HttpContext.User.Identity.Name, bindingContext.HttpContext.RequestAborted);
                if (userDocument == null)
                    throw new ArgumentNullException(nameof(userDocument));

                return userDocument;
            }
        }

        private IResponseCreator GetPropfindRequest(ModelBindingContext bindingContext)
        {
            try
            {
                XmlSerializer serializer = new(typeof(PropBody));
                if (bindingContext.ActionContext.HttpContext.Request.ContentLength == 0)
                    return new PropBody("allprop");
                var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body);

                var body = (IResponseCreator)serializer.Deserialize(reader);

                return body;
            }
            catch (InvalidOperationException)
            {
                throw new XmlDeserializeException("Incorrect xml");
            }
        }

        private IResponseCreator GetReportRequest(ModelBindingContext bindingContext)
        {
            try
            {
                using var ms = new MemoryStream();
                bindingContext.ActionContext.HttpContext.Request.Body.CopyTo(ms);
                ms.Position = 0;

                using var reader = new StreamReader(ms);

                var type = GetTypeByXml(reader);
                XmlSerializer serializer = new(type);

                var body = (IResponseCreator)serializer.Deserialize(reader);

                return body;
            }
            catch (InvalidOperationException)
            {
                throw new XmlDeserializeException("Incorrect xml");
            }
        }

        Type GetTypeByXml(StreamReader reader)
        {
            var xmlString = reader.ReadToEnd();
            reader.BaseStream.Position = 0;

            if (xmlString.Contains("addressbook-query"))
            {
                return typeof(AddresbookQueryBody);
            }
            else if (xmlString.Contains("addressbook-multiget"))
            {
                return typeof(MultigetBody);
            }
            else throw new ArgumentException("Unknown xml request");
        }
        #endregion
    }
}
