using System;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore
{
    public class MetroAudience : Audience<string>
    {
        public MetroAudience()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}