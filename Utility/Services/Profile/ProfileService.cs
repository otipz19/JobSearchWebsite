using Data;
using Data.Entities;
using Data.Entities.Base;
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
    }
}
