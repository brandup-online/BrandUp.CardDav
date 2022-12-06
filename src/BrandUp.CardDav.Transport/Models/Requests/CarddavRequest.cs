namespace BrandUp.CardDav.Transport.Models.Requests
{
    public class CarddavRequest
    {
        public string Depth { get; set; }

        public string ETag { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public string XmlContent { get; set; }
    }

    public static class Depth
    {
        public static string Zero => "0";
        public static string One => "1";
        public static string Infinity => "infinity";
    }
}
