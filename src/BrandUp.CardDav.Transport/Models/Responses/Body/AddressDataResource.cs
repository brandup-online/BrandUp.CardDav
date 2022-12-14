using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Models.Responses.Body
{
    internal class AddressDataResource : DefaultResponseResource
    {
        public VCardModel CardModel
        {
            get
            {
                if (FoundProperties.TryGetValue(new DefaultProp("address-data", "urn:ietf:params:xml:ns:carddav"), out var val))
                    return VCardParser.Parse(val);
                return null;
            }
        }
    }
}
