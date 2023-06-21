using Data.Entities;

namespace Utility.ViewModels
{
    public class ResumeDetailsVm
    {
        public Resume Resume { get; set; }

        public IEnumerable<Vacancie> AvailableVacancies { get; set; }

        public int? SelectedVacancieId { get; set; }

        public string Message { get; set; }
    }
}
