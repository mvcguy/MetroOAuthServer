using System;
using System.Data.Entity;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MetroOAuth.Jwt.MetroLib.Config;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore;
using Microsoft.Owin.Security;

namespace MetroOAuth.Jwt.MetroLib.Provider
{
    public class MetroJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _issuer;
        private readonly IAudienceStore<MetroAudience> _audienceStore;
        private const string AudiencePropertyKey = "audience";

        public MetroJwtFormat(string issuer, IAudienceStore<MetroAudience> audienceStore)
        {
            _issuer = issuer;
            _audienceStore = audienceStore;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data.Properties.Dictionary.ContainsKey(AudiencePropertyKey))
            {
                var audience = data.Properties.Dictionary[AudiencePropertyKey];
                if (!string.IsNullOrWhiteSpace(audience))
                {
                    var find = _audienceStore.FindById(audience);
                    if (find != null)
                    {

                        var store = new X509Store(OAuthConfig.Config.CertStore, OAuthConfig.Config.CertLocation);
                        store.Open(OpenFlags.ReadOnly);
                        try
                        {
                            var matches = store.Certificates.Find(OAuthConfig.Config.CertFindType, OAuthConfig.Config.CertFindValue, false);

                            if (matches.Count == 0)
                            {
                                throw new Exception("Certificate cannot be found");
                            }
                            var x509 = matches[0];
                            var x509SigningCredentials = new X509SigningCredentials(x509);

                            var issued = data.Properties.IssuedUtc;
                            var expires = data.Properties.ExpiresUtc;
                            if (issued != null && expires != null)
                            {
                                var token = new JwtSecurityToken(_issuer, audience, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, x509SigningCredentials);
                                var handler = new JwtSecurityTokenHandler();
                                var jwt = handler.WriteToken(token);
                                return jwt;
                            }
                        }
                        finally
                        {
                            store.Close();
                        }
                    }
                }
            }

            throw new InvalidOperationException("invalid audience");
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}