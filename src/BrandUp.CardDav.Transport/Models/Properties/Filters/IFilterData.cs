using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Properties.Filters
{
    public interface IFilterData : IDavProperty
    {
        public string PropName { get; set; }
        public FilterMatchType Type { get; set; }
        IEnumerable<TextMatch> Conditions { get; }
    }
}