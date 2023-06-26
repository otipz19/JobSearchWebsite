using Data.Entities;
using Data.Entities.Base;
using Utility.Services.FilterServices;

namespace Utility.Interfaces.FilterServices
{
    public interface IFilterService<T>
        where T : BaseFilterableEntity
	{
        public IQueryable<T> ApplyFilter(IQueryable<T> query, VacancieResumeFilter filter);

        public Task<VacancieResumeFilter> PopulateFilter(VacancieResumeFilter filter);
	}
}
