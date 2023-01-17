using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Responses.Body;

namespace BrandUp.CardDav.Transport.Handling
{
    /// <summary>
    /// 
    /// </summary>
    public class EtagHandler : IPropertyHandler
    {
        private IDavProperty prop = Prop.ETag;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressBook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IResourceBody> HandleAddressBookAsync(AddressBook addressBook, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = prop, IsFound = false } as IResourceBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IResourceBody> HandleContactAsync(Contact contact, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = prop, IsFound = false, Value = contact.ETag } as IResourceBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IResourceBody> HandlePrincipalAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = prop, IsFound = false } as IResourceBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IResourceBody> HandleUserAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = prop, IsFound = false } as IResourceBody);
        }
    }
}
