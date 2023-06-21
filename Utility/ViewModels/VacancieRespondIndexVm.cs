using Data.Entities;

namespace Utility.ViewModels
{
    public class VacancieRespondIndexVm
    {
        public List<VacancieRespondDetailsVm> VacancieRepsonds { get; set; }

        public Vacancie CommonVacancie { get; set; }

        public Resume CommonResume { get; set; }
    }
}
