using BrandUp.CardDav.Transport.Models.Requests;
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

            var serializer = new XmlSerializer(typeof(PropfindRequest));

            var reader = new StreamReader(bindingContext.ActionContext.HttpContext.Request.Body);


            var headers = bindingContext.ActionContext.HttpContext.Request.Headers;
            var model = (PropfindRequest)serializer.Deserialize(reader);

            model.Headers = headers.ToDictionary(h => h.Key, h => h.Value.ToString());

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}
