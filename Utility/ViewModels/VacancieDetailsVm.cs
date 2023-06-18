using Data.Entities;

namespace Utility.ViewModels
{
	public class VacancieDetailsVm
	{
        public Vacancie Vacancie { get; set; }

        public IEnumerable<Resume> AvailableResumes { get; set; }

        public int SelectedResumeId { get; set; }
    }
}
