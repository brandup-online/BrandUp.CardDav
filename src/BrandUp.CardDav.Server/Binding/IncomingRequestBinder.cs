using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Requests;
using BrandUp.CardDav.Transport.Abstract.Responces;
using BrandUp.CardDav.Transport.Models.Properties;
using BrandUp.CardDav.Transport.Models.Requests.Body.Mkcol;
using BrandUp.CardDav.Transport.Models.Requests.Body.Propfind;
using BrandUp.CardDav.Transport.Models.Requests.Body.Report;
using BrandUp.CardDav.Xml;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace BrandUp.CardDav.Transport.Server.Binding
{
    public class IncomingRequestBinder : IModelBinder
    {
        readonly ILogger<IncomingRequestBinder> logger;
        readonly IHandlerContext handlerContext;

        public IncomingRequestBinder(IHandlerContext handlerContext, ILogger<IncomingRequestBinder> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.handlerContext = handlerContext ?? throw new ArgumentNullException(nameof(handlerContext));
        }

        #region IModelBinder member

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                logger.LogWarning($"Binding incoming request:");

                logger.LogWarning($"User: {bindingContext.HttpContext.User.Identity.Name}");

                var method = bindingContext.HttpContext.Request.Method;

                if (bindingContext.HttpContext.Request.Headers.ContainsKey("Depth"))
                    logger.LogWarning(bindingContext.HttpContext.Request.Headers["Depth"]);

                logger.LogWarning($"Method: {method}");
                IRequestBody body = null;

                using var ms = new MemoryStream();

                if (bindingContext.HttpContext.Request.ContentLength > 0)
                {
                    bindingContext.ActionContext.HttpContext.Request.Body.CopyTo(ms);

                    await ms.FlushAsync();
                    ms.Position = 0;

                    using var reader = new StreamReader(ms);
                    var logstring = reader.ReadToEnd();
                    logger.LogWarning($"{logstring}");
                    ms.Position = 0;

                    body = await CustomSerializer.DeserializeRequestAsync(ms);

                    ValidateBody(body, bindingContext.ModelState);
                    if (!bindingContext.ModelState.IsValid)
                    {
                        logger.LogError(string.Join("\r\n", bindingContext.ModelState.Select(_ => _.Value.Errors.Select(e => e.ErrorMessage))));
                        return;
                    }
                }

                if (body == null)
                {
                    logger.LogWarning("Empty xml request!");
                }

                var flag = false;
                if (body is PropBody propBody)
                {
                    flag = propBody.IsAllProp;
                }

                var handlerList = GetHandlersForRequest(body, flag);

                IBodyWithFilter filter = null;
                if (body is IBodyWithFilter)
                    filter = (IBodyWithFilter)body;

                var incomingRequest = new IncomingRequest()
                {
                    Handlers = handlerList,
                    Endpoint = bindingContext.HttpContext.Request.Path,
                    Filter = filter,
                    IsAllProp = flag
                };

                bindingContext.Result = ModelBindingResult.Success(incomingRequest);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogError(ex.Message);

                bindingContext.ModelState.AddModelError("Handlers", ex.Message);
            }
        }

        #endregion

        #region Helpers

        private IDictionary<IDavProperty, IPropertyHandler> GetHandlersForRequest(IRequestBody body, bool flag)
        {
            if (flag)
                return handlerContext.All();

            var result = new Dictionary<IDavProperty, IPropertyHandler>();

            foreach (var prop in body.Properties)
            {
                var handler = handlerContext.GetHandler(prop);
                if (handler != null)
                {
                    result.Add(prop, handler);
                }
                else throw new ArgumentNullException($"Unexpected property: {prop.Namespace}:{prop.Name}");
            }

            return result;
        }

        void ValidateBody(IRequestBody body, ModelStateDictionary modelState)
        {
            var comparer = new PropertyComparer();

            if (body is PropBody propfind)
            {
                if (propfind.IsAllProp && propfind.Properties.Any())
                    modelState.AddModelError("Body", "allprop is not only one property in request");
                if (propfind.Properties.Contains(new AddressData(), comparer))
                    modelState.AddModelError("Body", "Address-data in propfind request");

            }

            if (body is MultigetBody multiget)
            {
                if (!multiget.VCardEndpoints.Any())
                    modelState.AddModelError("Body", "Address-data in propfind request");

            }

            if (body is AddresbookQueryBody addresbookQuery)
            {

            }

            if (body is SetPropBody setProp)
            {

            }
        }

        private class PropertyComparer : IEqualityComparer<IDavProperty>
        {
            public bool Equals(IDavProperty x, IDavProperty y) => x.Name == y.Name && x.Namespace == y.Namespace;

            public int GetHashCode([DisallowNull] IDavProperty obj) => $"{obj.Namespace}:{obj.Name}".GetHashCode();
        }

        #endregion
    }
}
