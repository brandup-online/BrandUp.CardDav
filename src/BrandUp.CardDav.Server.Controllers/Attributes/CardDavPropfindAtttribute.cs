namespace BrandUp.CardDav.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class CardDavPropfindAttribute : CardDavAttribute
    {
        public CardDavPropfindAttribute() : base("PROPFIND", "text/xml", "application/xml")
        { }
        public CardDavPropfindAttribute(string template) : base("PROPFIND", template, "text/xml", "application/xml")
        { }
    }
}
