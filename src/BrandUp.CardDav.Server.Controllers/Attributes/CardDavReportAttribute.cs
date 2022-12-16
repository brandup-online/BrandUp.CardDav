namespace BrandUp.CardDav.Server.Attributes
{
    public sealed class CardDavReportAttribute : CardDavAttribute
    {
        public CardDavReportAttribute() : base("REPORT", "text/xml", "application/xml")
        {
        }

        public CardDavReportAttribute(string template) : base("REPORT", template, "text/xml", "application/xml")
        {
        }
    }
}
