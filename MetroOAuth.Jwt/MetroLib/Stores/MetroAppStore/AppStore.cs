using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAppStore
{
    public class AppStore<TMetroApp> : IAppStore<TMetroApp> where TMetroApp:MetroApp
    {
        private readonly DbContext _dbContext;
        private readonly EntityStore<TMetroApp> _apps;

        public IQueryable<TMetroApp> Apps
        {
            get { return _apps.DbEntitySet; }
        }

        public AppStore(DbContext dbContext)
        {
            _dbContext = dbContext;
            _apps = new EntityStore<TMetroApp>(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task CreateAsync(TMetroApp app)
        {
             if (app == null)
                throw new ArgumentNullException("app");
            _apps.Create(app);
            await SaveChanges();
        }

        public async Task UpdateAsync(TMetroApp app)
        {
             if (app == null)
                throw new ArgumentNullException("app");
            _apps.Update(app);
            await SaveChanges();
        }

        public async Task DeleteAsync(TMetroApp app)
        {
             if (app == null)
                throw new ArgumentNullException("app");
            _apps.Delete(app);
            await SaveChanges();
        }

        public async Task<TMetroApp> FindByIdAsync(string appId)
        {
            return await Apps.FirstOrDefaultAsync(x => x.Id == appId);
        }

        private async Task SaveChanges()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}