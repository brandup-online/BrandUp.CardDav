﻿using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Responses.Body;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Models.Responses
{

    public class PropfindResponse : IResponse
    {
        public bool IsSuccess { get; init; }
        public string StatusCode { get; init; }

        public ResponseBody Body { get; init; }

        IResponseBody IResponse.Body => Body;

        public static PropfindResponse Create(HttpResponseMessage message)
        {
            var serializer = new XmlSerializer(typeof(ResponseBody));

            var body = (ResponseBody)serializer.Deserialize(message.Content.ReadAsStream());
            var response = new PropfindResponse { IsSuccess = message.IsSuccessStatusCode, StatusCode = message.StatusCode.ToString(), Body = body };

            return response;
        }
    }
}
