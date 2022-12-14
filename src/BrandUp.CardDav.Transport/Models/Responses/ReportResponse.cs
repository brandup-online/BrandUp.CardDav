using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class ReportResponse : IResponse
    {
        public ReportResponse() { }

        #region IResponse member

        public bool IsSuccess { get; set; }

        public string StatusCode { get; set; }

        public IResponseBody Body { get; set; }

        #endregion
    }
}
