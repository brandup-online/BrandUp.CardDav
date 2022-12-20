using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    public interface IFilterData : IDavProperty
    {
        public string PropName { get; }
        public FilterMatchType Type { get; }
        IEnumerable<TextMatch> Conditions { get; }
    }
}