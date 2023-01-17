using BrandUp.CardDav.Transport.Abstract.Properties;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Abstract.Responces
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
        /// 
        /// </summary>
        IEnumerable<IResourceBody> Resources { set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<IDavProperty, string> FoundProperties { get; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IDavProperty> NotFoundProperties { get; }
    }
}
