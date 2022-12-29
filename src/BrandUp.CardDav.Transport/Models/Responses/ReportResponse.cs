using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportResponse : IResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public ReportResponse() { }

        #region IResponse member

        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public int StatusCode { get; init; }

        /// <summary>
        /// 
        /// </summary>
        public ReportResponseBody Body { get; init; }
        IResponseBody IResponse.Content => Body;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
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
