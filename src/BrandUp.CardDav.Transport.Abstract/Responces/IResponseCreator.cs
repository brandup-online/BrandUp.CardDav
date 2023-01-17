using BrandUp.CardDav.Server.Abstractions;

namespace BrandUp.CardDav.Transport.Abstract.Responces
{
    /// <summary>
    /// Type can create response body
    /// </summary>
    public interface IBodyWithFilter
    {
        /// <summary>
        /// Filtering collection.
        /// </summary>
        /// <param name="collection"> documents collection </param>
        /// <returns>Body of response <see cref="IResponseBody"/> Filtered collection </returns>

        IEnumerable<IDavDocument> FilterCollection(IEnumerable<IDavDocument> collection);
    }
}
