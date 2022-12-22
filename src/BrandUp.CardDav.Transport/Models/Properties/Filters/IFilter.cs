using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.VCard;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    public interface IFilter : IDavProperty
    {
        public VCardProperty PropName { get; }
        public FilterMatchType Type { get; }
        IEnumerable<TextMatch> Conditions { get; }

        bool CheckConditions(VCardModel vCardModel);
    }
}