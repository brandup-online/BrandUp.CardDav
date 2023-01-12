using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters.Conditions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICondition : IDavProperty
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Check(VCardLine value);
    }
}