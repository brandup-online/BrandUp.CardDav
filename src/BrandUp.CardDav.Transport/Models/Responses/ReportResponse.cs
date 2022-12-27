using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    public class ReportResponse : IResponse
    {
        public ReportResponse() { }

        #region IResponse member

        public bool IsSuccess { get; init; }

        public int StatusCode { get; init; }

        public ReportResponseBody Body { get; init; }
        IResponseBody IResponse.Content => Body;

        public static IResponse Create(HttpResponseMessage message)
        {
            var serializer = new XmlSerializer(typeof(ReportResponseBody));

            var body = (ReportResponseBody)serializer.Deserialize(message.Content.ReadAsStream());
            var response = new ReportResponse { IsSuccess = message.IsSuccessStatusCode, StatusCode = ((int)message.StatusCode), Body = body };

            return response;
        }

        #endregion
    }
}
