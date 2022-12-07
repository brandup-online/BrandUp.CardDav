using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    public class AddressBookRequest : IReportRequest
    {
        public IDictionary<string, string> Headers => throw new NotImplementedException();

        public IRequestBody Body => throw new NotImplementedException();

        public HttpRequestMessage ToHttpRequest()
        {
            throw new NotImplementedException();
        }
    }
}
