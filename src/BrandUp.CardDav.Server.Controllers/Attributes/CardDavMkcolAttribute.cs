namespace BrandUp.CardDav.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class CardDavMkcolAttribute : CardDavAttribute
    {
        public CardDavMkcolAttribute() : base("MKCOL", contentType: "text/xml", "application/xml")
        { }
        public CardDavMkcolAttribute(string template) : base("MKCOL", template, "text/xml", "application/xml")
        { }
    }
}
