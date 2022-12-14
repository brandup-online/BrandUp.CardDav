using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    /// <summary>
    /// Determine CardDav property
    /// </summary>
    public interface IDavProperty : IXmlSerializable
    {
        /// <summary>
        /// Local name of property
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Xml namespase of property
        /// </summary>
        string Namespace { get; }
    }
}