namespace BrandUp.Carddav.Client.Models.Requests
{
    public class CarddavRequest
    {
        //TODO: переделать в енам
        //0, 1, infinity
        public string Depth { get; set; }

        public string XmlContent { get; set; }
    }
}
