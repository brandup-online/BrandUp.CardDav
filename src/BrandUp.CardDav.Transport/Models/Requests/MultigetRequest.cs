using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    public class MultigetRequest : IReportRequest
    {
        public IDictionary<string, string> Headers { get; set; }

        public IRequestBody Body { get; set; }

        public HttpRequestMessage ToHttpRequest()
        {
            throw new NotImplementedException();
        }
    }
}
