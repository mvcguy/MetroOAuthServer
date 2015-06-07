# MetroOAuthServer

The Metro OAuth server will simplify your work of issuing JWT (JSON Web Tokens) to your clients. All you need to do is to 

1. Register it with your IAppBuilder.

        public void Configuration(IAppBuilder appBuilder)
        {
            ConfigureAuth(appBuilder);
            appBuilder.UseOAuthAuthorizationServer(new MetroOAuthServerOptions(new ApplicationDbContext()));
        }
        
2. Add a section to your web.config file:
      
        <configSections>
          <section name="metroOAuthConfig" type="MetroOAuth.Jwt.MetroLib.Config.OAuthConfig" />
        </configSections>

3. Define the section in your web.config as follows:

        <metroOAuthConfig
            certLocation="LocalMachine"
            certStore="My" 
            certFindValue="MetroOAuthServer"
            certFindType="FindByIssuerName"
            tokenPath="/token"
            accessTokenExpireTimeSpan="60.0"
            allowInsecureHttp="true"
            issuer="http://localhost" >
        </metroOAuthConfig>
