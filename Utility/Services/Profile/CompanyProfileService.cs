using Data.Entities;
using Data;
using Utility.Interfaces.Profile;

namespace Utility.Services.Profile
{
    public class CompanyProfileService : ProfileService<Company>, ICompanyProfileService
    {
        public CompanyProfileService(AppDbContext appDbContext) : base(appDbContext) { }

        public async Task<Company> CreateProfile(AppUser user)
        {
            var profile = new Company()
            {
                AppUserId = user.Id,
                Name = user.UserName,
            };
            _dbContext.Companies.Add(profile);
            await _dbContext.SaveChangesAsync();
            return profile;
        }
    }
}
