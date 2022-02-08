using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SectorControlApi.Constants;
using SectorControlApi.Services;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SectorControlApi.Security
{
    public class UserAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        IUserService _userService;
        public UserAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserService userService) : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string authorizationHeader = Request.Headers["Authorization"];

            if(!authorizationHeader.Contains("Bearer"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string token = authorizationHeader.Substring("bearer".Length).Trim();

            return await ValidateTokenAsync(token);

        }

        private async Task<AuthenticateResult> ValidateTokenAsync(string token)
        {

            int? userId = await _userService.VerifyTokenAsync(token);
            if (userId == null)
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            var claims = new List<Claim>
                {
                //enum with type
                    new Claim(CustomClaimTypes.UserId, userId.ToString()),
                };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Principal.GenericPrincipal(identity, null);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
