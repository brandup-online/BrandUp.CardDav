using BrandUp.CardDav.Server.Abstractions.Documents;

namespace BrandUp.CardDav.Transport.Models.Abstract
{
    /// <summary>
    /// Type can create 
    /// </summary>
    public interface IResponseCreator
    {
        /// <summary>
        /// Creates response based on data collection and object fields.
        /// </summary>
        ///<param name="collection"> keys - document endpoint, value - document value </param>
        /// <returns>Body of response <see cref="IResponseBody"/>. Made based on request object/></returns>

        IResponseBody CreateResponse(IDictionary<string, IDavDocument> collection);
    }
}
