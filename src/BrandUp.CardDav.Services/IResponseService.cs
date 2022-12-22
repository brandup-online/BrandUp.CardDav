using BrandUp.CardDav.Attributes;
using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using BrandUp.CardDav.VCard;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace BrandUp.CardDav.Services
{
    public class ResponseService : IResponseService
    {
        readonly IUserRepository userRepository;
        readonly IContactRepository contactRepository;
        readonly IAddressBookRepository addressRepository;
        readonly string path;

        public ResponseService(IUserRepository userRepository, IContactRepository contactRepository, IAddressBookRepository addressRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            this.addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));

            path = httpContextAccessor.HttpContext.Request.Path;
        }

        readonly static PropertyInfo[] userProperties;
        readonly static PropertyInfo[] addressbookProperties;
        readonly static PropertyInfo[] contactProperties;


        static ResponseService()
        {
            userProperties = typeof(IUserDocument).GetProperties();
            addressbookProperties = typeof(IAddressBookDocument).GetProperties();
            contactProperties = typeof(IContactDocument).GetProperties();
        }

        #region IResponseService

        public Task<IUserDocument> FindUserAsync(string name, CancellationToken cancellationToken)
            => userRepository.FindByNameAsync(name, cancellationToken);

        public Task<IUserDocument> FindUserAsync(Guid id, CancellationToken cancellationToken)
            => userRepository.FindByIdAsync(id, cancellationToken);


        public async Task<IAddressBookDocument> FindAddressBookAsync(string name, string addressBookName, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByNameAsync(name, cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var addresBook = await addressRepository.FindByNameAsync(addressBookName, user.Id, cancellationToken);

            if (addresBook == null)
                throw new ArgumentNullException(nameof(user));

            return addresBook;
        }

        public Task<PropfindResponseBody> ProcessPropfindAsync<T>(T document, PropfindRequest request, CancellationToken cancellationToken)
        {
            if (document is IUserDocument user)
            {
                return PropfindUserAsync(user, request, cancellationToken);
            }
            else if (document is IAddressBookDocument book)
            {
                return PropfindAddressBookAsync(book, request, cancellationToken);
            }
            else if (document is IContactDocument contact)
            {
                return PropfindContactAsync(contact, request, cancellationToken);
            }
            else throw new ArgumentException("Unknow type");
        }

        public async Task<ReportResponseBody> ProcessReportAsync(IAddressBookDocument addressBookDocument, ReportRequest request, CancellationToken cancellationToken)
        {
            var contacts = await contactRepository.FindAllContactsByBookIdAsync(addressBookDocument.Id, cancellationToken);

            contacts = ApplyConstraints(contacts, request.Body);

            var response = new ReportResponseBody();

            foreach (var contact in contacts)
            {
                response.Resources.Add(GenerateReportResource(contact, request.Body.Properties, true));
            }

            return response;
        }

        public async Task<bool> MakeCollectionAsync(string name, string addressBook, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByNameAsync(name, cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var book = await addressRepository.FindByNameAsync(addressBook, user.Id, cancellationToken);

            if (book != null)
                throw new ArgumentException(nameof(book));

            await addressRepository.CreateAsync(addressBook, user.Id, cancellationToken);

            return true;
        }

        #endregion

        #region Helpers

        async Task<PropfindResponseBody> PropfindUserAsync(IUserDocument user, PropfindRequest request, CancellationToken cancellationToken)
        {
            var response = new PropfindResponseBody();

            response.Resources.Add(GenerateResponseResource(user, request.Body.Properties));

            if (request.Depth.Value == Depth.One.Value)
            {
                var addresBooks = await addressRepository.FindCollectionsByUserIdAsync(user.Id, cancellationToken);
                foreach (var book in addresBooks)
                {
                    response.Resources.Add(GenerateResponseResource(book, request.Body.Properties, true));
                }
            }

            return response;
        }

        async Task<PropfindResponseBody> PropfindAddressBookAsync(IAddressBookDocument book, PropfindRequest request, CancellationToken cancellationToken)
        {
            var response = new PropfindResponseBody();

            response.Resources.Add(GenerateResponseResource(book, request.Body.Properties));

            if (request.Depth.Value == Depth.One.Value)
            {
                var contacts = await contactRepository.FindAllContactsByBookIdAsync(book.Id, cancellationToken);
                foreach (var contact in contacts)
                {
                    response.Resources.Add(GenerateResponseResource(contact, request.Body.Properties, true));
                }
            }

            return response;
        }

        Task<PropfindResponseBody> PropfindContactAsync(IContactDocument contact, PropfindRequest request, CancellationToken cancellationToken)
        {
            var response = new PropfindResponseBody();

            response.Resources.Add(GenerateResponseResource(contact, request.Body.Properties));

            return Task.FromResult(response);
        }

        object GetValueByDavProp(object obj, IDavProperty davProperty)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var properties = GetPropertiesByObject(obj);

            var property = (from prop in properties
                            from attrib in prop.GetCustomAttributes(typeof(DavNameAttribute), true).Cast<DavNameAttribute>()
                            where attrib.Name == davProperty.Name && attrib.Namespace == davProperty.Namespace
                            select prop).FirstOrDefault();

            if (property == null)
                return null;

            return property.GetValue(obj);
        }

        AddressDataResource GenerateReportResource(IContactDocument contact, IEnumerable<IDavProperty> davProperties, bool withResourceName = false)
        {
            var endpoint = path;

            if (withResourceName)
                endpoint = string.Join('/', endpoint, contact.Name);

            Dictionary<IDavProperty, string> propertyDictionary = new();
            List<IDavProperty> notFound = new();

            foreach (var property in davProperties)
            {
                var value = (string)GetValueByDavProp(contact, property);

                if (property is AddressData address)
                {

                    if (address.VCardProperies.Any())
                    {
                        var vCard = VCardParser.Parse(value);
                        value = vCard.ToStringProps(address.VCardProperies);
                    }
                }

                if (value == null)
                    notFound.Add(property);
                else if (!propertyDictionary.TryAdd(property, value))
                    continue;
            }

            return new()
            {
                Endpoint = endpoint,
                FoundProperties = new(propertyDictionary),
                NotFoundProperties = notFound,
            };
        }

        DefaultResponseResource GenerateResponseResource(object innerResource, IEnumerable<IDavProperty> davProperties, bool withResourceName = false)
        {
            var endpoint = path;

            if (withResourceName)
                endpoint = string.Join('/', endpoint, (string)GetPropertiesByObject(innerResource).FirstOrDefault(p => p.Name == "Name").GetValue(innerResource));

            Dictionary<IDavProperty, string> propertyDictionary = new();
            List<IDavProperty> notFound = new();

            foreach (var property in davProperties)
            {
                var value = (string)GetValueByDavProp(innerResource, property);

                if (value == null)
                    notFound.Add(property);
                else if (!propertyDictionary.TryAdd(property, value))
                    continue;
            }

            return new()
            {
                Endpoint = endpoint,
                FoundProperties = new(propertyDictionary),
                NotFoundProperties = notFound
            };
        }


        private IEnumerable<IContactDocument> ApplyConstraints(IEnumerable<IContactDocument> contacts, IReportBody body)
        {
            return body.FillterCollection(contacts);
        }

        static PropertyInfo[] GetPropertiesByObject(object obj)
        {
            var type = obj.GetType();

            if (type.IsAssignableTo(typeof(IUserDocument)))
                return userProperties;
            if (type.IsAssignableTo(typeof(IAddressBookDocument)))
                return addressbookProperties;
            if (type.IsAssignableTo(typeof(IContactDocument)))
                return contactProperties;

            throw new ArgumentException("Unknown type");
        }

        #endregion
    }

    public interface IResponseService
    {
        public Task<IUserDocument> FindUserAsync(string name, CancellationToken cancellationToken);
        public Task<IUserDocument> FindUserAsync(Guid id, CancellationToken cancellationToken);
        public Task<IAddressBookDocument> FindAddressBookAsync(string name, string addressBookName, CancellationToken cancellationToken);
        public Task<PropfindResponseBody> ProcessPropfindAsync<T>(T document, PropfindRequest request, CancellationToken cancellationToken);
        public Task<ReportResponseBody> ProcessReportAsync(IAddressBookDocument addressBookDocument, ReportRequest request, CancellationToken cancellationToken);
        public Task<bool> MakeCollectionAsync(string name, string addressBook, CancellationToken cancellationToken);
    }
}