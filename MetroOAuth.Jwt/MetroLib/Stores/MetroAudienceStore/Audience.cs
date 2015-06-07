using MetroOAuth.Jwt.MetroLib.Models;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAppStore;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore
{
    public class Audience<TKey>:IAudience<TKey>
    {
        /// <summary>
        /// Client Id
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        /// Secret key
        /// </summary>
        public virtual string Base64Key { get; set; }

        /// <summary>
        /// Key issued to User
        /// </summary>
        public virtual MetroApplicationUser User { get; set; }

        /// <summary>
        /// Key issued for application
        /// </summary>
        public virtual MetroApp Application { get; set; }
    }
}