using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Properties.Filters.Conditions;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFilterProperty : IDavProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public CardProperty PropName { get; }

        /// <summary>
        /// 
        /// </summary>
        public FilterMatchType Type { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<ICondition> Conditions { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vCardModel"></param>
        /// <returns></returns>
        bool CheckConditions(VCardModel vCardModel);
    }
}