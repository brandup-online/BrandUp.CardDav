namespace BrandUp.CardDav.Server.Attributes
{
    /// <summary>
    /// Defines an action with Report method.
    /// </summary>
    public sealed class CardDavReportAttribute : CardDavAttribute
    {
        /// <summary>
        /// Contructor
        /// </summary>
        public CardDavReportAttribute() : base("REPORT", contentType: "text/xml", "application/xml")
        {
        }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="template">Route template</param>
        public CardDavReportAttribute(string template) : base("REPORT", template, "text/xml", "application/xml")
        {
        }
    }
}
