using Utility.Services.FilterServices;

namespace Utility.ViewModels
{
    public class VacancieIndexListVm
	{
        public IEnumerable<VacancieIndexVm> Vacancies { get; set; }

        public VacancieFilter Filter { get; set; }

        public int TotalVacancies { get; set; }

        public int? CurrentPage { get; set; }

        public bool IsPreviousDisabled { get; set; }

        public bool IsNextDisabled { get; set; }
    }
}
