namespace BrandUp.CardDav.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CardDavPropfindAttribute : CardDavAttribute
    {
        public CardDavPropfindAttribute() : base("PROPFIND", contentType: "text/xml", "application/xml")
        { }
        public CardDavPropfindAttribute(string template) : base("PROPFIND", template, "text/xml", "application/xml")
        { }
    }
}
