using BrandUp.CardDav.Transport.Abstract.Handling;
using BrandUp.CardDav.Transport.Abstract.Properties;
using BrandUp.CardDav.Transport.Abstract.Responces;
using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Transport.Server.Binding
{
    [ModelBinder(typeof(IncomingRequestBinder))]
    public class IncomingRequest
    {
        public IDictionary<IDavProperty, IPropertyHandler> Handlers { get; init; }

        public string Endpoint { get; init; }

        public IBodyWithFilter Filter { get; init; }

        public bool IsAllProp { get; init; }
    }
}
