# MetroOAuthServer - [Nuget] (https://www.nuget.org/packages/MetroOAuth/ "Nuget")

The Metro OAuth server will simplify your work of issuing JWT (JSON Web Tokens) to your clients. All you need to do is to 

1. Register it with your IAppBuilder.
```csharp
public void Configuration(IAppBuilder appBuilder)
{
    ConfigureAuth(appBuilder);
    appBuilder.UseOAuthAuthorizationServer(new MetroOAuthServerOptions(new ApplicationDbContext()));
}
```  
2. Add a section to your web.config file:
```xml      
        <configSections>
          <section name="metroOAuthConfig" type="MetroOAuth.Jwt.MetroLib.Config.OAuthConfig" />
        </configSections>
```
3. Define the section in your web.config as follows:
```xml
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
 ```    
## Registering Apps and Audience ##
The metro server provides implementation for two key components of your applications.

1. Apps - Your API's and applications you want to secure
2. Audience - To issue client_ids and secret_keys to registered audiences

### REMEMBER: ###

The Metro server extends IdentityDbContext, and to take benefits of the two modules, i.e., Apps & Audience your applicaton DbContext must extends the Metro Db Context. The MetroDbContext looks like this:
```csharp
        public class MetroDbContext<TUser> : IdentityDbContext<TUser> where TUser : MetroApplicationUser
        {
                ...
        }
```
So it is important to remember that your ApplicationDbContext must extends the Metro db context in the following way:
```csharp
        public class ApplicationDbContext:MetroDbContext<MetroApplicationUser>
        {
                ...
        }
```
### Creating admin user, audience and Apps ###

I am providing a seed method implementation here, which comes very handy to bootstrap your application with some initial data.
```csharp
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
```
## Once an audience is registered ##
Then all you need to do is to request the JWT from the MetroServer. The metro server will authenticate you against the Identity database, and will issue you the token. The Metro server requires you to provide a valid X509 certificate through the configuration as shown in preceeding section.

I am using fiddler for now to do a post to the server in the following way:

        URL: http://localhost/MetroOAuthServer/token
        User-Agent: Fiddler
        Content-Type: application/x-www-form-urlencoded
        
        POST Data/payload:
                
        username=username@example.com&password=YourStrongPassword&grant_type=password&client_Id=62b26683-6062-4a27-9e2c-60b83499d406&client_Secret=NSgaZpfIRLCews_4f3YHrCsq10v6UX0NoWTjvtkYlZUonumv5bFZ3aF2ygvm56Ht6MhwNU6ZI3QzBgdipSMcPQ

If everything goes as expected, the MetroServer will respond with a token as follows:
```json
        {"access_token":"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IkpVLXFTd0diU2pTMDdSVVozRXViRXI4N2FhNCJ9.eyJ1bmlxdWVfbmFtZSI6Im1lLnNoYWhpZGFsaUB5YWhvby5jb20iLCJzdWIiOiJtZS5zaGFoaWRhbGlAeWFob28uY29tIiwicm9sZSI6ImFkZEFwcCIsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3QiLCJhdWQiOiI2MmIyNjY4My02MDYyLTRhMjctOWUyYy02MGI4MzQ5OWQ0MDYiLCJleHAiOjE0MzM3MDU3MTUsIm5iZiI6MTQzMzcwMjExNX0.00gowfXkyA5D5jze0f5sqDH4hJ5s0K1mzL-KXfMjCWXmPYFSraJk31X2EFTL0QI1gN05N1I1gJ95CULc_hU0IU_CKFHq-T639S3K-k12MMz7TaqgCrnUE1LiBH-4HkbH27gyBsOpIjqxJ56WnSl-Nk5dwObGGHWbizN9m9guT2rWF5H07fvbYzvTieUN6BHYi46-J7dg0Wir7wE0A0s2DfYIlV6C1dG6pWBYwYXMB0FqmdY7_GbYBAdRp6SgZxmNyDdXoT5xDV0eBAaTxkXv22szWDiXBv6pdJrU3GOCaU-fa5Bnzc62WBnsuu4icA6o_PQovAPtwme11pZTM046o07RgiOxafP2oLI4SWr0BCeIjqH3jjWwXc-eH2lzWu3cUHwJXcexve-ovzyAu47mQAHckN7kJmOsfKpw-c9bSkVVvqWRLkrR0ykOrcP3_6fNL7sdcRu7DcDUlGgGtVjNVblDHROEzPApEr9LMEv4LmTAz8ecg5TVIXEpxu_U95Aj5jFHaDik_aIWjk3tkK4Cwp_NgJerVei8jem5pjCI--m17X1cJSknVqA1IhL_d8mm0Bv7n3JeZ5a7ih6b7fFbefkoPn7dIrs4KFihYRj_1FMDYHP117KKXkBDf18m8UxmORHZvm3RYgfU72C2IVfOP6RP7fAYS92TqObpXsManB8","token_type":"bearer","expires_in":3599}
```        
### [Nuget] (https://www.nuget.org/packages/MetroOAuth/ "Nuget") ###

        PM> Install-Package MetroOAuth 

