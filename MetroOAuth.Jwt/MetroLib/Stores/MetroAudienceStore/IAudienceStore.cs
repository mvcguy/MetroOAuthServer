using System;
using System.Threading.Tasks;

namespace MetroOAuth.Jwt.MetroLib.Stores.MetroAudienceStore
{
    public interface IAudienceStore<TAudience> : IDisposable where TAudience : class, IAudience<string>
    {
        Task CreateAsync(TAudience audience);

        Task UpdateAsync(TAudience audience);

        Task DeleteAsync(TAudience audience);

        Task<TAudience> FindByIdAsync(string audienceId);

        TAudience FindById(string audienceId);
    }
}