using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace vILTConnector.API
{
    /// <summary>
    /// class to handle basic authentication.
    /// </summary>
    /// <remarks>
    /// Addapted from https://jasonwatmore.com/post/2019/10/21/aspnet-core-3-basic-authentication-tutorial-with-example-api#basic-authentication-handler-cs
    /// </remarks>
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// scheme name for authentication handler.
        /// </summary>
        public const string SchemeName = "BasicAuthentication";
        private readonly string BasicUsername;
        private readonly string BasicPassword;
        private readonly string AppName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="config">allows injection of key vault configuration</param>
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration config)
            : base(options, logger, encoder, clock)
        {
            BasicPassword = config["BasicPassword"]; //pulls from Azure Key Vault
            BasicUsername = config["BasicUsername"]; //pulls from Azure Key Vault
            AppName = config["ApplicationName"]; //pulls from appsettings.json

        }


        /// <summary>
        /// verify that require authorization header exists and handle decode data.
        /// </summary>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];
                

                if (username != BasicUsername
                    &&
                   password != BasicPassword
                )
                {
                    return AuthenticateResult.Fail("Bad Username or Password");
                }
            }
            catch(Exception exception)
            {
                return AuthenticateResult.Fail($"Invalid Authorization Header. Exception: {exception.Message}");
            }

             //pulls from appsettings.json

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, AppName),
                new Claim(ClaimTypes.Name, AppName),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
