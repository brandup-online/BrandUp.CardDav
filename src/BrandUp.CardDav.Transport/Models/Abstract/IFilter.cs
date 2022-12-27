namespace BrandUp.CardDav.Transport.Models.Abstract
{
    public interface IFilter
    {
        public IEnumerable<T> FillterCollection<T>(IEnumerable<T> collection);
    }
}