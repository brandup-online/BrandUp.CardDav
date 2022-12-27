namespace BrandUp.CardDav.Services.Exceptions
{
    public class DavPropertyException : Exception
    {
        public DavPropertyException(Exception inner) : base("Error when convertiong to a Dav property.", inner)
        { }
    }
}
