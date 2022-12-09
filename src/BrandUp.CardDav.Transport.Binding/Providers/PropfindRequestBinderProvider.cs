using BrandUp.CardDav.Transport.Models.Requests;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace BrandUp.CardDav.Transport.Binding.Providers
{
    public class PropfindRequestBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(PropfindRequest))
            {
                return new BinderTypeModelBinder(typeof(PropfindRequestBinder));
            }

            return null;
        }
    }
}
