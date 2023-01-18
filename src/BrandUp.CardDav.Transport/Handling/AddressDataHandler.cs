using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Models.Responses.Body;

namespace BrandUp.CardDav.Transport.Handling
{
    /// <summary>
    /// 
    /// </summary>
    public class AddressDataHandler : IPropertyHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public IDavProperty Property { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressBook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IResourceBody> HandleAddressBookAsync(AddressBook addressBook, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = Property, IsFound = false } as IResourceBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IResourceBody> HandleContactAsync(Contact contact, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = Property, IsFound = true, Value = contact.RawVCard } as IResourceBody); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IResourceBody> HandlePrincipalAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = Property, IsFound = false } as IResourceBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IResourceBody> HandleUserAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = Property, IsFound = false } as IResourceBody);
        }
    }
}
