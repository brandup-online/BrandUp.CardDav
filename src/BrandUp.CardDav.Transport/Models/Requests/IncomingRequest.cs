using BrandUp.CardDav.Server.Abstractions.Documents;
using BrandUp.CardDav.Transport.Models.Abstract;

namespace BrandUp.CardDav.Transport.Models.Requests
{
    public class IncomingRequest
    {
        public IDavDocument Document { get; init; }

        public IRequestBody Body { get; init; }

        public string Endpoint { get; init; }
    }
}
