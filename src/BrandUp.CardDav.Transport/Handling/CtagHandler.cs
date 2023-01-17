using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Server.Repositories;
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
    public class CtagHandler : IPropertyHandler
    {
        private IDavProperty prop = Prop.CTag;

        readonly IAddressBookRepository addressBookRepository;
        readonly IUserRepository userRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressBookRepository"></param>
        /// <param name="userRepository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CtagHandler(IAddressBookRepository addressBookRepository, IUserRepository userRepository)
        {
            this.addressBookRepository = addressBookRepository ?? throw new ArgumentNullException(nameof(addressBookRepository));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(addressBookRepository));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressBook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IResourceBody> HandleAddressBookAsync(AddressBook addressBook, CancellationToken cancellationToken)
        {
            var cTag = await addressBookRepository.GetCTagAsync(addressBook.Id, cancellationToken);

            if (cTag != null)
                return new ResourceBody { DavProperty = prop, IsFound = true, Value = cTag.Ctag };
            else
                return new ResourceBody { DavProperty = prop, IsFound = false };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IResourceBody> HandleContactAsync(Contact contact, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResourceBody { DavProperty = prop, IsFound = false } as IResourceBody);
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
        public async Task<IResourceBody> HandleUserAsync(User user, CancellationToken cancellationToken)
        {
            var cTag = await userRepository.GetCTagAsync(user.Id, cancellationToken);

            if (cTag != null)
                return new ResourceBody { DavProperty = prop, IsFound = true, Value = cTag.Ctag };
            else
                return new ResourceBody { DavProperty = prop, IsFound = false };
        }
    }
}
