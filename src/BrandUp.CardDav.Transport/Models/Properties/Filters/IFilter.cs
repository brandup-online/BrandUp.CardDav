using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFilter : IDavProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public VCardProperty PropName { get; }

        /// <summary>
        /// 
        /// </summary>
        public FilterMatchType Type { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<TextMatch> Conditions { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vCardModel"></param>
        /// <returns></returns>
        bool CheckConditions(VCardModel vCardModel);
    }
}