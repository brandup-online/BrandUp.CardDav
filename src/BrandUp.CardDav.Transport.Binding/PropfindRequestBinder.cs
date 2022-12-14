using BrandUp.CardDav.Transport.Models.Requests;
using BrandUp.CardDav.Transport.Models.Requests.Body.Propfind;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Xml.Serialization;

namespace BrandUp.CardDav.Transport.Binding
{
    public class PropfindRequestBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            XmlSerializer serializer = new(typeof(PropBody));

            var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body);

            var headers = bindingContext.ActionContext.HttpContext.Request.Headers;
            var body = (PropBody)serializer.Deserialize(reader);

            var model = new PropfindRequest(headers.ToDictionary(h => h.Key, h => h.Value.ToString()))
            {
                Body = body
            };

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}
