using System;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAppStore
{
    public class MetroApp : App<string>
    {
        public MetroApp()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}