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
        
## Registering Apps and Audience ##
The metro server provides implementation for two key components of your applications.

1. Apps - Your API's and applications you want to secure
2. Audience - To issue client_ids and secret_keys to registered audiences

### REMEMBER: ###

The Metro server extends IdentityDbContext, and to take benefits of the two modules, i.e., Apps & Audience your applicaton DbContext must extends the Metro Db Context. The MetroDbContext looks like this:

        public class MetroDbContext<TUser> : IdentityDbContext<TUser> where TUser : MetroApplicationUser
        {
                ...
        }

So it is important to remember that your ApplicationDbContext must extends the Metro db context in the following way:

        public class ApplicationDbContext:MetroDbContext<MetroApplicationUser>
        {
                ...
        }

### Creating admin user, audience and Apps ###

I am providing a seed method implementation here, which comes very handy to bootstrap your application with some initial data.

        public class AppMigrationConfig : DbMigrationsConfiguration<MetroDbContext<MetroApplicationUser>>
        {
            public AppMigrationConfig()
            {
                this.AutomaticMigrationsEnabled = true;
                #if DEBUG
                                this.AutomaticMigrationDataLossAllowed = true;
                #endif
            }

            protected override void Seed(MetroDbContext<MetroApplicationUser> context)
            {
                var userStore = new UserStore<MetroApplicationUser>(context);
                var userMgr = new ApplicationUserManager(userStore);

                var role = new IdentityRole("Admin");
                context.Roles.AddOrUpdate(x => x.Name, role);
                context.SaveChanges();

                var appAdmin = userMgr.Users.FirstOrDefault(x => x.Email == "username@exmaple.com");

                if (appAdmin == null)
                {
                    appAdmin = new MetroApplicationUser()
                    {
                        Name = "Admin Name",
                        JoinedOn = DateTime.Now,
                        Email = "username@exmaple.com",
                        UserName = "username@exmaple.com",
                    };
                    var result = userMgr.Create(appAdmin, "YourStrongPassword");
                }

                var adminRoles = userMgr.GetRoles(appAdmin.Id);

                if (!adminRoles.Contains(role.Name))
                {
                    var userRole = new IdentityUserRole
                    {
                        RoleId = role.Id,
                        UserId = appAdmin.Id
                    };
                    appAdmin.Roles.Add(userRole);
                }

                var blogsApp = new MetroApp()
                {
                    Name = "Metro Blogs"
                };

                var smsApp = new MetroApp()
                {
                    Name = "Metro SMS Service"
                };

                context.Apps.AddOrUpdate(x => x.Name, blogsApp, smsApp);
                context.SaveChanges();
                
                var audienceStore = new AudienceStore<MetroAudience>(context);
                var audienceManager = new AudienceManager(audienceStore);
           
                MetroAudience audience=null;
                try
                {
                        audience=Task.Run(() =>
                        {
                            var result = audienceManager.CreateAudienceForAppAsync(appAdmin, blogsApp);
                            return result;
                        }).Result;
                
                }
                catch (Exception e)
                {
                        var message = e.Message;
                }
            }
        }
