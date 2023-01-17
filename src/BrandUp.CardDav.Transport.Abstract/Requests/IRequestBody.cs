using BrandUp.CardDav.Transport.Abstract.Properties;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Abstract.Requests
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

        string Name { get; }

        string Namespace { get; }
    }
}
