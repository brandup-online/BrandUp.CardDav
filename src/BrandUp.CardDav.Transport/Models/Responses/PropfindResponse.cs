using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class PropfindResponse : IResponse
    {
        public bool IsSuccess { get; init; }
        public string StatusCode { get; init; }

        IEnumerable<IResponseResource> IResponse.Resources => Resources;

        public PropfindResponseResource[] Resources { get; init; }
        public static PropfindResponse Create(HttpResponseMessage message)
        {
            var resourses = XmlSerializer.DeserializeToResoure(message.Content.ReadAsStream());
            return new() { IsSuccess = message.IsSuccessStatusCode, StatusCode = message.StatusCode.ToString(), Resources = (PropfindResponseResource[])resourses.ToArray() };
        }
    }

    public class PropfindResponseResource : IResponseResource
    {
        public string Endpoint { get; }

        public IReadOnlyDictionary<IDavProperty, string> FoundProperties { get; }
    }
}
