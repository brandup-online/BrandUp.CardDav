using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    /// <summary>
    /// 
    /// </summary>
    public class AddressDataResource : DefaultResponseResource
    {
        /// <summary>
        /// 
        /// </summary>
        public VCardModel CardModel
        {
            get
            {
                if (FoundProperties.TryGetValue(new DefaultProp("address-data", "urn:ietf:params:xml:ns:carddav"), out var val))
                    return new VCardModel(val);
                return null;
            }
        }
    }
}
