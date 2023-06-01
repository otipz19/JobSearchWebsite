using Data.Entities;
using Data.Entities.Base;

namespace Utility.Interfaces.Profile
{
    public interface IProfileService<T>
        where T : BaseProfileEntity
    {
        public Task<T> CreateProfile(AppUser user);
    }
}
