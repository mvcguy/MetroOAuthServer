using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MetroOAuth.Jwt.MetroLib.Models;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace MetroOAuth.Jwt.MetroLib.Provider
{
    public class MetroOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IAudienceStore<MetroAudience> _audienceStore;
        private readonly UserManager<MetroApplicationUser> _userManager;

        public MetroOAuthAuthorizationServerProvider(IAudienceStore<MetroAudience> audienceStore, UserManager<MetroApplicationUser> userManager)
        {
            _audienceStore = audienceStore;
            _userManager = userManager;
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == context.UserName);
            //verify user credentials against user store
            if (user == null || !_userManager.CheckPassword(user, context.Password))
            {
                context.SetError("invalid credentials", "username or password is invalid");
                return Task.FromResult<object>(null);
            }


            //if verified, create claims identity and add claims
            var identity = new ClaimsIdentity();
            var roles = _userManager.GetRoles(user.Id);
            var claims = _userManager.GetClaims(user.Id);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim("sub", user.UserName));

            //add roles
            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            //add claims
            foreach (var claim in claims)
            {
                identity.AddClaim(new Claim(claim.Type, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer));
            }

            //these props will be available to the protect method of MetroJwtFormat class later on
            var props = new AuthenticationProperties(new Dictionary<string, string>()
            {
                {
                    "audience",context.ClientId??string.Empty
                }
            });

            //issue authentication ticket
            var ticket = new AuthenticationTicket(identity, props);

            //and confirm context validated
            context.Validated(ticket);
            return Task.FromResult<object>(null);
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            //verify client Id and client secret against audience store
            //mark context as validated on successful verification

            string clientId;
            string clientSecret;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (string.IsNullOrWhiteSpace(context.ClientId) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                context.SetError("invalid client_id or client_secret", "client_id or client_secret is not set");
                return;
            }

            var client = await _audienceStore.FindByIdAsync(clientId);
            if (client == null || client.Base64Key != clientSecret)
            {
                context.SetError("invlaid client_id", "client_id or client_secret is not valid");
                return;
            }

            context.Validated(clientId);

        }
    }
}