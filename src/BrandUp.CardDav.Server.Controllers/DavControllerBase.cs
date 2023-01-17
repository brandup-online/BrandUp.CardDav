using BrandUp.CardDav.Server.Abstractions;
using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrandUp.CardDav.Server
{
    public abstract class DavControllerBase : ControllerBase
    {
        readonly protected IUserRepository userRepository;
        readonly protected IAddressBookRepository addressBookRepository;
        readonly protected IContactRepository contactRepository;

        readonly protected ILogger logger;

        public DavControllerBase(IUserRepository userRepository, IAddressBookRepository adddressBookRepository, IContactRepository contactRepository, ILogger logger)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.addressBookRepository = adddressBookRepository ?? throw new ArgumentNullException(nameof(adddressBookRepository));
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task<ResponseResource> ProccessRessposeResourseAsync(IDictionary<IDavProperty, IPropertyHandler> handlers, string endpoint, IDavDocument davDocument, CancellationToken cancellationToken)
        {
            var responseBodyList = new List<IResourceBody>();

            foreach (var pair in handlers)
            {
                var responseBody = await InvokeHandlerAsync(pair.Value, davDocument, cancellationToken);

                responseBodyList.Add(responseBody);
            }

            return new() { Endpoint = endpoint, Resources = responseBodyList };
        }

        private Task<IResourceBody> InvokeHandlerAsync(IPropertyHandler propertyHandler, IDavDocument davDocument, CancellationToken cancellationToken)
        {
            if (davDocument == null)
                return propertyHandler.HandlePrincipalAsync(cancellationToken);

            if (davDocument is User user)
                return propertyHandler.HandleUserAsync(user, cancellationToken);

            if (davDocument is AddressBook addressBook)
                return propertyHandler.HandleAddressBookAsync(addressBook, cancellationToken);

            if (davDocument is Contact contact)
                return propertyHandler.HandleContactAsync(contact, cancellationToken);

            throw new NotSupportedException("Unexpexted type");
        }
    }
}
