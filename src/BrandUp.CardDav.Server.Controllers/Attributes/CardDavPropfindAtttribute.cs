namespace BrandUp.CardDav.Server.Attributes
{
    /// <summary>
    /// Defines an action with Propfind method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CardDavPropfindAttribute : CardDavAttribute
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public CardDavPropfindAttribute() : base("PROPFIND", contentType: "text/xml", "application/xml")
        { }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="template">Route template</param>
        public CardDavPropfindAttribute(string template) : base("PROPFIND", template, "text/xml", "application/xml")
        { }
    }
}
