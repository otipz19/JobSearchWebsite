using Data;
using Data.Entities;
using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility.Interfaces.Profile;

namespace Utility.Services.Profile
{
    public abstract class ProfileService<T>
        where T : BaseProfileEntity
    {
        protected readonly AppDbContext _dbContext;

        protected ProfileService(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task<bool> UserOwnsProfile(ClaimsPrincipal user, T profile)
        {
            T userProfile =  await GetUserProfile(user);
            return userProfile != null && userProfile.Id == profile.Id;
        }

        public async Task<T> GetUserProfile(ClaimsPrincipal user)
        {
            return await _dbContext.Set<T>()
                .FirstOrDefaultAsync(p => p.AppUserId == user.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        protected string GetImageSource(T profile, string defaultSrc) 
        {
            string src = profile.ImagePath;
            if (string.IsNullOrEmpty(src))
            {
                src = defaultSrc;
            }
            return src;
        }
    }
}
