using Data.Entities;

namespace Utility.ViewModels
{
    public class VacancieDetailsVm
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int LeftSalaryFork { get; set; }

        public int RightSalaryFork { get; set; }

        public int SphereId { get; set; }

        public int SpecializationId { get; set; }

        public int RemotenessId { get; set; }

        public int ExperienceLevelId { get; set; }

        public int EnglishLevelId { get; set; }

        public IEnumerable<Sphere> AvailableSpheres { get; set; }

        public IEnumerable<Specialization> AvailableSpecializations { get; set; }

        public IEnumerable<Remoteness> AvailableRemotenesses { get; set; }

        public IEnumerable<ExperienceLevel> AvailableExperienceLevels { get; set; }

        public IEnumerable<EnglishLevel> AvailableEnglishLevels { get; set; }

		public List<CheckboxOption> CheckboxKeywords { get; set; } = new();

		public List<CheckboxOption> CheckboxStates { get; set; } = new();

		public List<CheckboxOption> CheckboxCities { get; set; } = new();

        public List<string> SelectedKeywords { get; set; } = new();

		public List<string> SelectedStates { get; set; } = new();

		public List<string> SelectedCities { get; set; } = new();
	}
}
