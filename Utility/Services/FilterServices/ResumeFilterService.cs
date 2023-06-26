using Data;
using Data.Entities;
using Utility.Interfaces.Checkbox;
using Utility.Interfaces.FilterServices;

namespace Utility.Services.FilterServices
{
    public class ResumeFilterService : BaseFilterService<Resume>, IResumeFilterService
	{
		public ResumeFilterService(AppDbContext dbContext,
			ICheckboxService checkboxService) : base(dbContext, checkboxService)
		{
		}

		public override IQueryable<Resume> ApplyFilter(IQueryable<Resume> query, VacancieResumeFilter filter)
		{
			query = base.ApplyFilter(query, filter);

			if (filter.SalaryFrom.HasValue)
			{
				query = query.Where(r => r.WantedSalary >= filter.SalaryFrom);
			}

			if (filter.SalaryTo.HasValue)
			{
				query = query.Where(r => r.WantedSalary <= filter.SalaryTo);
			}

            if (filter.StatesId.Any())
            {
                List<int> ids = filter.StatesId.Select(int.Parse).ToList();
                query = query.Where(v => ids.Contains(v.StateId));
            }

            if (filter.CitiesId.Any())
            {
                List<int> ids = filter.CitiesId.Select(int.Parse).ToList();
                query = query.Where(v => ids.Contains(v.CityId));
            }

            return query;
		}
	}
}
