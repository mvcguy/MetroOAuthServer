using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MetroOAuth.Jwt.MetroLib.Models;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAppStore;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore
{
    public class AudienceManager
    {
        private readonly AudienceStore<MetroAudience> _audienceStore;

        public AudienceManager(AudienceStore<MetroAudience> audienceStore)
        {
            _audienceStore = audienceStore;
        }

        public virtual async Task<MetroAudience> CreateAudienceForAppAsync(MetroApplicationUser user, MetroApp application)
        {

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            if (AudienceForAppExists(user, application))
            {
                return await _audienceStore.Audiences.FirstOrDefaultAsync(x => x.User.Id == user.Id && x.Application.Id == application.Id);
            }

            if (string.IsNullOrWhiteSpace(application.Id))
            {
                application.Id = Guid.NewGuid().ToString();
            }

            byte[] key = new byte[64];
            RandomNumberGenerator.Create().GetBytes(key);
            var base64Key = TextEncodings.Base64Url.Encode(key);

            var audience = new MetroAudience()
            {
                Base64Key = base64Key,
                Application = application,
                User = user
            };
            await _audienceStore.CreateAsync(audience);

            return audience;
        }

        public bool AudienceForAppExists(MetroApplicationUser user, MetroApp app)
        {
            if (_audienceStore.Audiences.Count(x => x.User.Id == user.Id && x.Application.Id == app.Id) > 0)
            {
                return true;
            }
            return false;
        }

    }
}