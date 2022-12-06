using BrandUp.CardDav.Server.Core.Command;
using BrandUp.CardDav.Server.Repositories;
using BrandUp.Commands;
using System.ComponentModel.DataAnnotations;

namespace BrandUp.CardDav.Server.Core.Queries
{
    public class PropfindPrincipalCommand : ICommand<XmlResult>
    {
        [Required]
        public string PrincipalName { get; set; }
        [Required]
        public string XmlQuery { get; set; }
        public string Depth { get; set; } = "0";
    }

    public class PropfindPrincipalHandler : ICommandHandler<PropfindPrincipalCommand, XmlResult>
    {
        readonly IXmlTranslator xmlTranslator;
        readonly IUserRepository userRepository;
        readonly IAddressBookRepository addressBookRepository;
        public PropfindPrincipalHandler(IXmlTranslator xmlTranslator, IUserRepository userRepository, IAddressBookRepository addressBookRepository)
        {
            this.xmlTranslator = xmlTranslator ?? throw new ArgumentNullException(nameof(xmlTranslator));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public Task<Result<XmlResult>> HandleAsync(PropfindPrincipalCommand command, CancellationToken cancelationToken = default)
        {
            var properties = xmlTranslator.GetProperties();

            var
        }
    }
}
