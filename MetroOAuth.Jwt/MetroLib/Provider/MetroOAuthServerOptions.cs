using System;
using System.Data.Entity;
using MetroOAuth.Jwt.MetroLib.Config;
using MetroOAuth.Jwt.MetroLib.Models;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;

namespace MetroOAuth.Jwt.MetroLib.Provider
{
    public class MetroOAuthServerOptions : OAuthAuthorizationServerOptions
    {
        public MetroOAuthServerOptions(DbContext dbContext)
        {
            var audienceStore = new AudienceStore<MetroAudience>(dbContext);
            var userStore = new UserStore<MetroApplicationUser>(dbContext);
            var userManager = new UserManager<MetroApplicationUser>(userStore);


            this.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(OAuthConfig.Config.AccessTokenExpirationMinutes);
            this.AccessTokenFormat = new MetroJwtFormat(OAuthConfig.Config.Issuer, audienceStore);
            this.AllowInsecureHttp = OAuthConfig.Config.AllowInsecureHttp;
            this.Provider = new MetroOAuthAuthorizationServerProvider(audienceStore, userManager);
            this.TokenEndpointPath = new PathString(OAuthConfig.Config.TokenPath);
        }
    }
}