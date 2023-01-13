using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Abstract.Properties
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