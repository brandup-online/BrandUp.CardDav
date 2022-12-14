using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Requests.Body.Report;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    public class ReportRequest : ICardDavRequest
    {
        public ReportRequest()
        {

        }

        #region Static members

        public static ReportRequest CreateQuery()
            => new ReportRequest()
            {
                Body = new AddresbookQueryBody()
            };


        public static ReportRequest CreateMultiget()
            => new ReportRequest()
            {
                Body = new MultigetBody()
            };


        #endregion

        #region ICardDavRequest members

        public IDictionary<string, string> Headers { get; init; }

        public IRequestBody Body { get; init; }

        public HttpRequestMessage ToHttpRequest()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
