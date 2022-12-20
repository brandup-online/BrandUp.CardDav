using BrandUp.CardDav.Transport.Models.Abstract;
using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Requests.Body.Report;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Binding
{
    public class ReportRequestBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            using var ms = new MemoryStream();
            bindingContext.ActionContext.HttpContext.Request.Body.CopyTo(ms);
            ms.Position = 0;

            using var reader = new StreamReader(ms);

            var type = GetTypeByXml(reader);
            XmlSerializer serializer = new(type);

            var headers = bindingContext.ActionContext.HttpContext.Request.Headers;
            var body = (IRequestBody)serializer.Deserialize(reader);

            var model = new ReportRequest()
            {
                Headers = headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                Body = body
            };

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }

        #region Helpers

        Type GetTypeByXml(StreamReader reader)
        {
            var xmlString = reader.ReadToEnd();
            reader.BaseStream.Position = 0;

            if (xmlString.Contains("addressbook-query"))
            {
                return typeof(AddresbookQueryBody);
            }
            else if (xmlString.Contains("addressbook-multiget"))
            {
                return typeof(MultigetBody);
            }
            else throw new ArgumentException("Unknown xml request");
        }

        #endregion
    }
}