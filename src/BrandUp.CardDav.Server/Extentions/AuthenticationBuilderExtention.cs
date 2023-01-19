using BrandUp.CardDav.Server.Common;
using BrandUp.CardDav.Server.Example.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace BrandUp.CardDav.Server.Extentions
{
    public static class AuthenticationBuilderExtention
    {
        public static AuthenticationBuilder AddCardDavAuthentication(this AuthenticationBuilder builder)
        {
            builder.AddScheme<DavAuthenticationOptions, DavAuthenticationHandler>(Constants.ServerName, options =>
            {

            });

            return builder;
        }
    }
}
