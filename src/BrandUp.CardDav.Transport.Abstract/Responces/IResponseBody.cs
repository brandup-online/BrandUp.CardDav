using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Abstract.Responces
{
    /// <summary>
    /// Body of the xml response
    /// </summary>
    public interface IResponseBody : IXmlSerializable
    {
        /// <summary>
        /// Responses that server return
        /// </summary>
        IList<IResponseResource> Resources { get; }
    }
}
