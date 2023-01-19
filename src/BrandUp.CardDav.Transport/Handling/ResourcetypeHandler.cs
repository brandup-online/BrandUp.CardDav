using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using System.Xml;

namespace BrandUp.CardDav.Transport.Handling
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourcetypeHandler : IPropertyHandler
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
            return Task.FromResult(new ResourceBody { DavProperty = Property, IsFound = false } as IResourceBody);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IResourceBody> HandlePrincipalAsync(CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();

            using var writer = XmlWriter.Create(ms, new XmlWriterSettings { Async = true, ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true });

            writer.WriteStartElement("collection", "DAV:");

            writer.WriteStartElement("addressbook", "urn:ietf:params:xml:ns:carddav");
            writer.WriteEndElement();

            writer.WriteEndElement();

            await writer.FlushAsync();
            ms.Position = 0;

            using var reader = new StreamReader(ms);

            var value = await reader.ReadToEndAsync();

            return new ResourceBody { DavProperty = Property, IsFound = true, Value = value };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IResourceBody> HandleUserAsync(User user, CancellationToken cancellationToken)
        {
            using var ms = new MemoryStream();

            using var writer = XmlWriter.Create(ms, new XmlWriterSettings { Async = true, ConformanceLevel = ConformanceLevel.Fragment, OmitXmlDeclaration = true });

            writer.WriteStartElement("collection", "DAV:");
            writer.WriteEndElement();

            await writer.FlushAsync();
            ms.Position = 0;

            using var reader = new StreamReader(ms);

            var value = await reader.ReadToEndAsync();

            return new ResourceBody
            {
                DavProperty = Property,
                IsFound = true,
                Value = value
            };
        }
    }
}
