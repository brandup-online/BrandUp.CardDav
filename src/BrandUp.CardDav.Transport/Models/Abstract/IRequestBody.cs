using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    /// <summary>
    /// Base body of request
    /// </summary>
    public interface IRequestBody : IXmlSerializable
    {
        /// <summary>
        /// Requesting properties
        /// </summary>
        IEnumerable<IDavProperty> Properties { get; }
    }
}
