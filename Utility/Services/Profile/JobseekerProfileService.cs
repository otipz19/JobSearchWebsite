using Data;
using Data.Entities;
using Utility.Interfaces.Profile;

namespace Utility.Services.Profile
{
    public class JobseekerProfileService : ProfileService<Jobseeker>, IJobseekerProfileService
    {
        public JobseekerProfileService(AppDbContext appDbContext) : base(appDbContext) { }

        public async Task<Jobseeker> CreateProfile(AppUser user)
        {
            var profile = new Jobseeker()
            {
                AppUserId = user.Id,
                Name = user.UserName,
            };
            _dbContext.Jobseekers.Add(profile);
            await _dbContext.SaveChangesAsync();
            return profile;
        }
    }
}
