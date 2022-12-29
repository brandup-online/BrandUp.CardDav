using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    /// <summary>
    /// Body of the response
    /// </summary>
    public interface IResponseBody : IXmlSerializable
    {
        /// <summary>
        /// Responses that server return
        /// </summary>
        IList<IResponseResource> Resources { get; }
    }
}
