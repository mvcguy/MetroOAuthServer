using System;
using System.Threading.Tasks;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAppStore
{
    public interface IAppStore<TMetroApp> : IDisposable where TMetroApp : class, IApp<string>
    {
        Task CreateAsync(TMetroApp app);

        Task UpdateAsync(TMetroApp app);

        Task DeleteAsync(TMetroApp app);

        Task<TMetroApp> FindByIdAsync(string appId);
    }
}