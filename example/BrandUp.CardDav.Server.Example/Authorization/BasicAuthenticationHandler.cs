﻿using BrandUp.CardDav.Server.Example.Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace BrandUp.CardDav.Server.Example.Authorization
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
    {
        readonly UserRepository userRepository;
        public BasicAuthenticationHandler(UserRepository userRepository,
            IOptionsMonitor<AuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Logger.LogWarning("Authentication request");


            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            var cancellationToken = Context.RequestAborted;

            if (AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authHeader))
            {
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var username = credentials[0];
                var password = credentials[1];

                Logger.LogWarning("Requesting user from db...");

                var user = await userRepository.FindDocumentByNameAsync(username, cancellationToken);
                if (user != null)
                    Logger.LogWarning($"{user.Name}, {user.Id}");
                if (user.Password != password)
                    return AuthenticateResult.Fail("Invalid Username or Password");

                Logger.LogWarning("Request successful.");

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            else return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }
}
