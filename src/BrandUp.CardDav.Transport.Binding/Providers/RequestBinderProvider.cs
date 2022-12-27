using BrandUp.CardDav.Transport.Models.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace BrandUp.CardDav.Transport.Binding.Providers
{
    public class RequestBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(IncomingRequest))
            {
                return new BinderTypeModelBinder(typeof(IncomingRequestBinder));
            }

            return null;
        }
    }
}
