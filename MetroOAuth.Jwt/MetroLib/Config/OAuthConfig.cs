using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace MetroOAuth.Jwt.MetroLib.Config
{
    public class OAuthConfig : ConfigurationSection
    {
        [ConfigurationProperty("tokenPath", IsRequired = false, DefaultValue = "/token")]
        public string TokenPath
        {
            get { return (string)this["tokenPath"]; }
            set { this["tokenPath"] = value; }
        }

        [ConfigurationProperty("accessTokenExpireTimeSpan", IsRequired = false, DefaultValue = 60.0)]
        public double AccessTokenExpirationMinutes
        {
            get { return (double)this["accessTokenExpireTimeSpan"]; }
            set { this["accessTokenExpireTimeSpan"] = value; }
        }

        [ConfigurationProperty("allowInsecureHttp", IsRequired = false, DefaultValue = false)]
        public bool AllowInsecureHttp
        {
            get { return (bool)this["allowInsecureHttp"]; }
            set { this["allowInsecureHttp"] = value; }
        }

        [ConfigurationProperty("issuer", IsRequired = false, DefaultValue = "http://localhost")]
        public string Issuer
        {
            get { return (string)this["issuer"]; }
            set { this["issuer"] = value; }
        }

        [ConfigurationProperty("certLocation", IsRequired = false, DefaultValue = StoreLocation.LocalMachine)]
        public StoreLocation CertLocation
        {
            get { return (StoreLocation)this["certLocation"]; }
            set { this["certLocation"] = value; }
        }

        [ConfigurationProperty("certStore", IsRequired = false, DefaultValue = StoreName.My)]
        public StoreName CertStore
        {
            get { return (StoreName)this["certStore"]; }
            set { this["certStore"] = value; }
        }



        [ConfigurationProperty("certFindType", IsRequired = false, DefaultValue = X509FindType.FindByIssuerName)]
        public X509FindType CertFindType
        {
            get { return (X509FindType)this["certFindType"]; }
            set { this["certFindType"] = value; }
        }

        [ConfigurationProperty("certFindValue", IsRequired = false)]
        public string CertFindValue
        {
            get { return (string)this["certFindValue"]; }
            set { this["certFindValue"] = value; }
        }

        public static OAuthConfig Config
        {
            get { return ConfigurationManager.GetSection("metroOAuthConfig") as OAuthConfig; }
        }
    }

}