namespace BrandUp.CardDav.Transport.Models.Abstract
{
    /// <summary>
    /// Filter for collection
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Filtring collection
        /// </summary>
        /// <typeparam name="T">Collection type</typeparam>
        /// <param name="collection"></param>
        /// <returns>Filtred collection</returns>
        public IEnumerable<T> FillterCollection<T>(IEnumerable<T> collection);
    }
}