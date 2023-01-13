using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Abstract.Responces;
using Microsoft.AspNetCore.Mvc;

namespace BrandUp.CardDav.Transport.Binding
{
    [ModelBinder(typeof(IncomingRequestBinder))]
    public class IncomingRequest
    {
        public IDavDocument Document { get; init; }

        public IResponseCreator Body { get; init; }

        public string Endpoint { get; init; }
    }
}
