using Data.Entities;
using Data.Entities.Base;
using System.Security.Claims;

namespace Utility.Interfaces.Profile
{
    public interface IProfileService<T>
        where T : BaseProfileEntity
    {
        public Task<T> CreateProfile(AppUser user);

        public Task<bool> UserOwnsProfile(ClaimsPrincipal user, T profile);

        public Task<T> GetUserProfile(ClaimsPrincipal user);

        public string GetImageSource(T profile);
    }
}
