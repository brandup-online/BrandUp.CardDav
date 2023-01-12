using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    /// <summary>
    /// Deserialized Xml response propstat <see href="https://www.rfc-editor.org/rfc/rfc4918#section-14.22"/>
    /// </summary>
    public interface IResponseResource : IXmlSerializable
    {
        /// <summary>
        /// Resourse URL
        /// </summary>
        public string Endpoint { get; }

        /// <summary>
        /// Properties that server return with value
        /// </summary>
        public PropertyDictionary FoundProperties { get; }

        /// <summary>
        /// Properties that server return without value
        /// </summary>
        public IEnumerable<IDavProperty> NotFoundProperties { get; }
    }
}
