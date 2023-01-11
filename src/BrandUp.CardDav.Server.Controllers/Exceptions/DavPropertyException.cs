namespace BrandUp.CardDav.Server.Exceptions
{
    /// <summary>
    /// Throws if deserializer can't convert xml to request object.
    /// </summary>
    public class DavPropertyException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inner"></param>
        public DavPropertyException(Exception inner) : base("Error when converting to a Dav property.", inner)
        { }
    }
}
