using BrandUp.CardDav.Attributes;
using BrandUp.CardDav.Server.Abstractions.Additional;
using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Services.Exceptions;
using BrandUp.CardDav.Transport.Binding;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Headers;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using BrandUp.CardDav.VCard;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;

namespace BrandUp.CardDav.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ResponseService : IResponseService
    {
        readonly IUserRepository userRepository;
        readonly IContactRepository contactRepository;
        readonly IAddressBookRepository addressRepository;

        readonly ILogger<ResponseService> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="contactRepository"></param>
        /// <param name="addressRepository"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ResponseService(IUserRepository userRepository, IContactRepository contactRepository, IAddressBookRepository addressRepository, ILogger<ResponseService> logger)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            this.addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region IResponseService

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="depth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Task<PropfindResponseBody> ProcessPropfindAsync(IncomingRequest request, string depth, CancellationToken cancellationToken)
        {
            if (request.Document is IUserDocument user)
            {
                return PropfindUserAsync(request, depth, cancellationToken);
            }
            else if (request.Document is IAddressBookDocument book)
            {
                return PropfindAddressBookAsync(request, depth, cancellationToken);
            }
            else if (request.Document is IContactDocument contact)
            {
                return PropfindContactAsync(request, depth, cancellationToken);
            }
            else throw new ArgumentException("Unknow type");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ReportResponseBody> ProcessReportAsync(IncomingRequest request, CancellationToken cancellationToken)
        {
            if (request.Document is IAddressBookDocument bookDocument)
            {
                var contacts = await contactRepository.FindAllContactsByBookIdAsync(bookDocument.Id, cancellationToken);

                if (request.Body is IReportBody body)
                    contacts = ApplyConstraints(contacts, body);
                else throw new ArgumentException("Unexpected type");

                var response = new ReportResponseBody();

                foreach (var contact in contacts)
                {
                    response.Resources.Add(GenerateReportResource(contact, request.Body.Properties, request.Endpoint, true));
                }

                return response;
            }
            else throw new ArgumentException("Unexpected type");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="addressBook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ConflictException"></exception>
        public async Task<bool> MakeCollectionAsync(string name, string addressBook, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByNameAsync(name, cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var book = await addressRepository.FindByNameAsync(addressBook, user.Id, cancellationToken);

            if (book != null)
                throw new ConflictException(book, addressBook);

            await addressRepository.CreateAsync(addressBook, user.Id, cancellationToken);

            return true;
        }

        #endregion

        #region Helpers

        //main properties
        readonly static PropertyInfo[] userProperties;
        readonly static PropertyInfo[] addressbookProperties;
        readonly static PropertyInfo[] contactProperties;

        //additional properties
        readonly static PropertyInfo[] cTagProperties;
        readonly static PropertyInfo[] SyncProperties;

        static ResponseService()
        {
            userProperties = typeof(IUserDocument).GetProperties();
            addressbookProperties = typeof(IAddressBookDocument).GetProperties();
            contactProperties = typeof(IContactDocument).GetProperties();
            cTagProperties = typeof(ICTag).GetProperties();
            SyncProperties = typeof(ISyncToken).GetProperties();
        }


        async Task<PropfindResponseBody> PropfindUserAsync(IncomingRequest request, string depth, CancellationToken cancellationToken)
        {
            var response = new PropfindResponseBody();

            response.Resources.Add(GeneratePropfindResource(request.Document, request.Body.Properties, request.Endpoint));

            if (depth == Depth.One.Value)
            {
                var addresBooks = await addressRepository.FindCollectionsByUserIdAsync(request.Document.Id, cancellationToken);
                foreach (var book in addresBooks)
                {
                    response.Resources.Add(GeneratePropfindResource(book, request.Body.Properties, request.Endpoint, true));
                }
            }

            return response;
        }

        async Task<PropfindResponseBody> PropfindAddressBookAsync(IncomingRequest request, string depth, CancellationToken cancellationToken)
        {
            var response = new PropfindResponseBody();

            response.Resources.Add(GeneratePropfindResource(request.Document, request.Body.Properties, request.Endpoint));

            if (depth == Depth.One.Value)
            {
                var contacts = await contactRepository.FindAllContactsByBookIdAsync(request.Document.Id, cancellationToken);
                foreach (var contact in contacts)
                {
                    response.Resources.Add(GeneratePropfindResource(contact, request.Body.Properties, request.Endpoint, true));
                }
            }

            return response;
        }

        Task<PropfindResponseBody> PropfindContactAsync(IncomingRequest request, string depth, CancellationToken cancellationToken)
        {
            var response = new PropfindResponseBody();

            response.Resources.Add(GeneratePropfindResource(request.Document, request.Body.Properties, request.Endpoint));

            return Task.FromResult(response);
        }

        AddressDataResource GenerateReportResource(IContactDocument contact, IEnumerable<IDavProperty> davProperties, string endpoint, bool withResourceName = false)
        {
            if (withResourceName)
                endpoint = string.Join('/', endpoint, contact.Name);

            Dictionary<IDavProperty, string> propertyDictionary = new();
            List<IDavProperty> notFound = new();

            foreach (var property in davProperties)
            {
                var value = GetValueByDavProp(contact, property);

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

        DefaultResponseResource GeneratePropfindResource(IDavDocument document, IEnumerable<IDavProperty> davProperties, string endpoint, bool withResourceName = false)
        {
            if (withResourceName)
                endpoint = string.Join('/', endpoint, document.Name);

            Dictionary<IDavProperty, string> propertyDictionary = new();
            List<IDavProperty> notFound = new();

            if (davProperties.Count() == 1 && davProperties.SingleOrDefault()?.Name == "allprop")
            {
                propertyDictionary = new(GetAllDavPropValues(document));
            }
            else
                foreach (var property in davProperties)
                {
                    var value = GetValueByDavProp(document, property);

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

        string GetValueByDavProp(object obj, IDavProperty davProperty)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            try
            {
                var properties = GetPropertiesByObject(obj);

                var property = (from prop in properties
                                from attrib in prop.GetCustomAttributes(typeof(DavNameAttribute), true).Cast<DavNameAttribute>()
                                where attrib.Name == davProperty.Name && attrib.Namespace == davProperty.Namespace
                                select prop).FirstOrDefault();

                if (property == null)
                    return null;

                var converter = TypeDescriptor.GetConverter(property.PropertyType);
                return converter.ConvertToString(property.GetValue(obj));
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                throw new DavPropertyException(ex);
            }
        }

        IDictionary<IDavProperty, string> GetAllDavPropValues(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            try
            {
                var properties = GetPropertiesByObject(obj);

                var davProperties = (from prop in properties
                                     from attrib in prop.GetCustomAttributes(typeof(DavNameAttribute), true).Cast<DavNameAttribute>()
                                     where attrib.Name != "address-data" && attrib.Namespace != "urn:ietf:params:xml:ns:carddav"
                                     select prop);

                return davProperties.ToDictionary(k =>
                {
                    var attr = ((DavNameAttribute)k.GetCustomAttribute(typeof(DavNameAttribute)));

                    return Prop.Create(attr.Name, attr.Namespace);
                }, v => (string)v.GetValue(obj));
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.Message);
                throw new DavPropertyException(ex);
            }
        }

        private IEnumerable<IContactDocument> ApplyConstraints(IEnumerable<IContactDocument> contacts, IReportBody body)
        {
            return body.FillterCollection(contacts);
        }

        static PropertyInfo[] GetPropertiesByObject(object obj)
        {
            var type = obj.GetType();
            var members = new List<PropertyInfo>();

            if (type.IsAssignableTo(typeof(IUserDocument)))
                members.AddRange(userProperties);
            if (type.IsAssignableTo(typeof(IAddressBookDocument)))
                members.AddRange(addressbookProperties);
            if (type.IsAssignableTo(typeof(IContactDocument)))
                members.AddRange(contactProperties);

            if (type.IsAssignableTo(typeof(ICTag)))
                members.AddRange(cTagProperties);
            if (type.IsAssignableTo(typeof(ISyncToken)))
                members.AddRange(SyncProperties);

            return members.ToArray();
        }

        #endregion
    }

    /// <summary>
    /// Creates responses for CardDav requests.
    /// </summary>
    public interface IResponseService
    {
        /// <summary>
        /// Creates response for Propfind request.
        /// </summary>
        /// <param name="request"><see cref="IncomingRequest" /></param>
        /// <param name="depth"><see href="https://www.rfc-editor.org/rfc/rfc4918#section-10.2" /></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<PropfindResponseBody> ProcessPropfindAsync(IncomingRequest request, string depth, CancellationToken cancellationToken);

        /// <summary>
        /// Creates response for Report request.
        /// </summary>
        /// <param name="request"><see cref="IncomingRequest" /></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ReportResponseBody> ProcessReportAsync(IncomingRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Creates Address book collection.
        /// </summary>
        /// <param name="name">User name</param>
        /// <param name="addressBook">Address book name</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> MakeCollectionAsync(string name, string addressBook, CancellationToken cancellationToken);
    }
}