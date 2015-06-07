using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using MetroOAuth.Jwt.MetroLib.Models;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAppStore;
using MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MetroOAuth.Jwt.MetroLib.Data
{
    public class MetroDbContext<TUser> : IdentityDbContext<TUser> where TUser : MetroApplicationUser
    {
        public IDbSet<MetroApp> Apps { get; set; }
        public IDbSet<MetroAudience> Audiences { get; set; }

        public MetroDbContext(): base("DefaultConnection"){}

        public MetroDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public MetroDbContext(string nameOrConnectionString, bool throwIfV1Schema):base(nameOrConnectionString,throwIfV1Schema){}

        public MetroDbContext(DbCompiledModel dbCompiledModel):base(dbCompiledModel){}

        public MetroDbContext(DbConnection existingConnection,DbCompiledModel dbCompiledModel,bool contextOwnsConnection):base(existingConnection,dbCompiledModel,contextOwnsConnection){}

        public MetroDbContext(DbConnection existingConnection,bool contextOwnsConnection):base(existingConnection,contextOwnsConnection){}
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            var usersConfig = new EntityTypeConfiguration<TUser>();
            usersConfig.HasMany(x => x.RegisteredApps).WithOptional(x => (TUser) x.User).WillCascadeOnDelete(false);

            var appsConfig = new EntityTypeConfiguration<MetroApp>();
            appsConfig.HasMany(x => x.AppAudiences).WithOptional(x => x.Application).WillCascadeOnDelete(false);

            appsConfig.Property(x => x.Name).IsRequired().HasMaxLength(150);
            usersConfig.Property(x => x.Name).HasMaxLength(80);

            modelBuilder.Configurations.Add(appsConfig).Add(usersConfig);
            base.OnModelCreating(modelBuilder);
        }

    }

    
}