using BrandUp.CardDav.Transport.Abstract.Enum;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Abstract.Properties
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