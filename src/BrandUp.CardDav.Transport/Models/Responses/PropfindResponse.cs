using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses
{
    /// <summary>
    /// 
    /// </summary>
    public class PropfindResponse : IResponse
    {
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
        public PropfindResponseBody Body { get; init; }

        IResponseBody IResponse.Content => Body;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IResponse Create(HttpResponseMessage message)
        {
            var serializer = new XmlSerializer(typeof(PropfindResponseBody));

            var body = (PropfindResponseBody)serializer.Deserialize(message.Content.ReadAsStream());
            var response = new PropfindResponse { IsSuccess = message.IsSuccessStatusCode, StatusCode = ((int)message.StatusCode), Body = body };

            return response;
        }
    }
}

