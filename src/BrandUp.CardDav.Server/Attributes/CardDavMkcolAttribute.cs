namespace BrandUp.CardDav.Server.Attributes
{
    /// <summary>
    /// Defines an action with Mkcol method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class CardDavMkcolAttribute : CardDavAttribute
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public CardDavMkcolAttribute() : base("MKCOL", contentType: "text/xml", "application/xml")
        { }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="template">Route template</param>
        public CardDavMkcolAttribute(string template) : base("MKCOL", template, "text/xml", "application/xml")
        { }
    }
}
