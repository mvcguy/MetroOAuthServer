using System.Collections.Generic;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAppStore
{
    public class App<TKey>:IApp<TKey>
    {
        public virtual TKey Id { get; set; }

        public virtual string Name { get; set; }

        public virtual ICollection<MetroAudience> AppAudiences { get; set; }
    }
}