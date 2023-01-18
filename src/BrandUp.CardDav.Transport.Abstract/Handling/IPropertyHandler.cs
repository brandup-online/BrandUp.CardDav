using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Responces;

namespace BrandUp.CardDav.Transport.Abstract.Handling
{
    public interface IPropertyHandler
    {
        IDavProperty Property { get; set; }

        Task<IResourceBody> HandlePrincipalAsync(CancellationToken cancellationToken);

        Task<IResourceBody> HandleUserAsync(User user, CancellationToken cancellationToken);

        Task<IResourceBody> HandleAddressBookAsync(AddressBook addressBook, CancellationToken cancellationToken);

        Task<IResourceBody> HandleContactAsync(Contact contact, CancellationToken cancellationToken);
    }
}
