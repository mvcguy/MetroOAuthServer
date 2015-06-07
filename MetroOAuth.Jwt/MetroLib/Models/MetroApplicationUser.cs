using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MetroOAuth.Jwt.MetroLib.Models
{
    public class MetroApplicationUser : IdentityUser
    {
        public virtual string Name { get; set; }

        public virtual DateTime? JoinedOn { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<MetroApplicationUser> manager)
        {
            ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, "ApplicationCookie");
            return userIdentity;
        }

        public virtual ICollection<MetroAudience> RegisteredApps { get; set; }
    }
}