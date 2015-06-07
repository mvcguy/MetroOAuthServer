using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MetroOAuth.Jwt.MetroLib.Stores
{
    public class EntityStore<TEntity> where TEntity : class
    {
        public DbContext Context { get; private set; }

        public IQueryable<TEntity> EntitySet
        {
            get
            {
                return DbEntitySet;
            }
        }

        public DbSet<TEntity> DbEntitySet { get; private set; }

        public EntityStore(DbContext context)
        {
            Context = context;
            DbEntitySet = context.Set<TEntity>();
        }

        public virtual Task<TEntity> GetByIdAsync(object id)
        {
            return DbEntitySet.FindAsync(id);
        }

        public void Create(TEntity entity)
        {
            DbEntitySet.Add(entity);
        }

        public void Delete(TEntity entity)
        {
            DbEntitySet.Remove(entity);
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null)
                return;
            Context.Entry(entity).State = EntityState.Modified;
        }
    }
}