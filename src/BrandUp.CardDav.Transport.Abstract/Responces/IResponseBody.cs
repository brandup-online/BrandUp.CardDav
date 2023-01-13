using BrandUp.CardDav.Transport.Abstract.Properties;

namespace BrandUp.CardDav.Transport.Abstract.Responces
{
    /// <summary>
    /// Body of the xml response
    /// </summary>
    public interface IResponseBody : IDavProperty
    {
        /// <summary>
        /// Responses that server return
        /// </summary>
        IList<IResponseResource> Resources { get; }
    }
}
