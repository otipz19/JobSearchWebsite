using Data.Entities;

namespace Utility.ViewModels
{
    public class CompanyDetailsVm
    {
        public Company Company { get; set; }

        public IEnumerable<VacancieIndexVm> Vacancies { get; set; }
    }
}
