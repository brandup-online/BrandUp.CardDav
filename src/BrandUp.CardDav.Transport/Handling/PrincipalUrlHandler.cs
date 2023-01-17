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
    public class PrincipalUrlHandler : IPropertyHandler
    {
        private const string principals = "/principals";
        private IDavProperty prop = Prop.PrincipalUrl;


        private Task<IResourceBody> Result => Task.FromResult(new ResourceBody { DavProperty = prop, IsFound = true, Value = principals } as IResourceBody);
        /// <summary>
        /// 
        /// </summary>


        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressBook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IResourceBody> HandleAddressBookAsync(AddressBook addressBook, CancellationToken cancellationToken)
        {
            return Result;
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
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IResourceBody> HandlePrincipalAsync(CancellationToken cancellationToken)
        {
            return Result;
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
            return Result;
        }


    }
}
