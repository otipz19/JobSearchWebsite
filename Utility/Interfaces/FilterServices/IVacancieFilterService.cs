using Data.Entities;
using Utility.Services.FilterServices;

namespace Utility.Interfaces.FilterServices
{
    public interface IVacancieFilterService
	{
		public IQueryable<Vacancie> ApplyFilter(VacancieFilter filter);

		public Task<VacancieFilter> PopulateFilter(VacancieFilter filter);
	}
}
