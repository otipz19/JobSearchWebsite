using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Utility.Interfaces.Checkbox;
using Utility.Interfaces.FilterServices;

namespace Utility.Services.FilterServices
{
    public class VacancieFilterService : BaseFilterService<Vacancie>, IVacancieFilterService
	{
		public VacancieFilterService(AppDbContext dbContext,
			ICheckboxService checkboxService) : base(dbContext, checkboxService)
		{
		}

		public override IQueryable<Vacancie> ApplyFilter(IQueryable<Vacancie> query, VacancieResumeFilter filter)
		{
			query = base.ApplyFilter(query, filter);

			if (filter.SalaryFrom.HasValue)
			{
				query = query.Where(v => v.LeftSalaryFork >= filter.SalaryFrom);
			}

			if (filter.SalaryTo.HasValue)
			{
				query = query.Where(v => v.RightSalaryFork <= filter.SalaryTo);
			}

			query.Include(v => v.Cities).Include(v => v.States);

			if (filter.CitiesId.Any())
			{
				List<int> ids = filter.CitiesId.Select(int.Parse).ToList();
				//v.Cities.Count(c => ids.Contains(c.Id)) > 0 - the way to check intersection of sets,
				//so it can be translated to SQL by EF Core
				query = query.Where(v => v.Cities.Count(c => ids.Contains(c.Id)) > 0);
			}

			if (filter.StatesId.Any())
			{
				List<int> ids = filter.StatesId.Select(int.Parse).ToList();
				query = query.Where(v => v.States.Count(s => ids.Contains(s.Id)) > 0);
			}

			return query;
		}
	}
}
