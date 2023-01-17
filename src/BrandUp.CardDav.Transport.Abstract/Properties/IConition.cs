using BrandUp.CardDav.VCard;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Abstract.Properties
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICondition : IXmlSerializable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Check(VCardLine value);
    }
}