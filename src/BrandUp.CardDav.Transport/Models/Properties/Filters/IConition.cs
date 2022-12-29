using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
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
        bool Check(string value);
    }
}