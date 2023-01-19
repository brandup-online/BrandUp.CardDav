using BrandUp.CardDav.Server.Common;
using Microsoft.AspNetCore.Authorization;

namespace BrandUp.CardDav.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    internal class ServerAuthorizeAttribute : AuthorizeAttribute
    {
        public ServerAuthorizeAttribute() => AuthenticationSchemes = Constants.ServerName;
    }
}
