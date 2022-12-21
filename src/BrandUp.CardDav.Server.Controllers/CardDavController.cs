using BrandUp.CardDav.Server.Documents;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.CardDav.Transport.Attributes;
using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using BrandUp.CardDav.VCard;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BrandUp.CardDav.Server.Controllers
{
    public abstract class CardDavController : ControllerBase
    {
        readonly protected IUserRepository userRepository;
        readonly protected IContactRepository contactRepository;
        readonly protected IAddressBookRepository addressRepository;

        protected CardDavController(IUserRepository userRepository, IContactRepository contactRepository, IAddressBookRepository addressRepository) : this()
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            this.addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
        }

        readonly PropertyInfo[] userProperties;
        readonly PropertyInfo[] addressbookProperties;
        readonly PropertyInfo[] contactProperties;


        private CardDavController()
        {
            userProperties = typeof(IUserDocument).GetProperties();
            addressbookProperties = typeof(IAddressBookDocument).GetProperties();
            contactProperties = typeof(IContactDocument).GetProperties();
        }

        #region Helpers

        protected object GetValueByDavProp(object obj, IDavProperty davProperty)
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


        protected DefaultResponseResource GenerateResponseResource(object innerResource, IEnumerable<IDavProperty> davProperties, bool withResourceName = false)
        {
            var endpoint = Request.Path.Value;

            if (withResourceName)
                endpoint = string.Join('/', endpoint, (string)GetPropertiesByObject(innerResource).FirstOrDefault(p => p.Name == "Name").GetValue(innerResource));

            Dictionary<IDavProperty, string> propertyDictionary = new();
            List<IDavProperty> notFound = new();

            foreach (var property in davProperties)
            {
                var value = (string)GetValueByDavProp(innerResource, property);

                //bad(((
                if (property is AddressData address)
                {
                    var vCard = VCardParser.Parse(value);
                    value = vCard.ToStringProps(address.VCardProperies);
                }

                if (value == null)
                    notFound.Add(property);
                else if (!propertyDictionary.TryAdd(property, value))
                    continue;
            }

            return new DefaultResponseResource
            {
                Endpoint = endpoint,
                FoundProperties = new(propertyDictionary),
                NotFoundProperties = notFound
            };
        }

        PropertyInfo[] GetPropertiesByObject(object obj)
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
}
