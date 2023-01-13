using BrandUp.CardDav.Transport.Abstract.Properties;

namespace BrandUp.CardDav.Transport.Abstract.Requests
{
    /// <summary>
    /// Base body of request
    /// </summary>
    public interface IRequestBody : IDavProperty
    {
        /// <summary>
        /// Requesting properties
        /// </summary>
        IEnumerable<IDavProperty> Properties { get; }
    }
}
