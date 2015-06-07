using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore
{
    public class AudienceStore<TAudience> : IAudienceStore<TAudience> where TAudience : MetroAudience
    {
        private readonly DbContext _context;
        private readonly EntityStore<TAudience> _audienceStore;

        public IQueryable<TAudience> Audiences
        {
            get { return _audienceStore.EntitySet; }
        }


        public AudienceStore(DbContext context)
        {
            _context = context;
            _audienceStore = new EntityStore<TAudience>(context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task CreateAsync(TAudience audience)
        {
            if (audience == null)
                throw new ArgumentNullException("audience");
            _audienceStore.Create(audience);
            await SaveChanges();
        }

        public async Task UpdateAsync(TAudience audience)
        {
            if (audience == null)
                throw new ArgumentNullException("audience");
            _audienceStore.Update(audience);
            await SaveChanges();
        }

        private async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TAudience audience)
        {
            if (audience == null)
                throw new ArgumentNullException("audience");
            _audienceStore.Delete(audience);
            await SaveChanges();
        }

        public async Task<TAudience> FindByIdAsync(string audienceId)
        {
            var audience = await Audiences.FirstOrDefaultAsync(x => x.Id == audienceId);
            return audience;
        }

        public TAudience FindById(string audienceId)
        {
            var audience = Audiences.FirstOrDefault(x => x.Id == audienceId);
            return audience;
        }
    }
}