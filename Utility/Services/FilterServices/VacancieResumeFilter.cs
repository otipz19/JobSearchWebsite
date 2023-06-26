using Data.Entities;
using Utility.Services.Checkbox;

namespace Utility.Services.FilterServices
{
    public class VacancieResumeFilter
    {
        public string SearchQuery { get; set; }

        public int? SalaryFrom { get; set; }

        public int? SalaryTo { get; set; }


        public List<string> StatesId { get; set; } = new();

        public IEnumerable<CheckboxOption> CheckboxStates { get; set; }


        public List<string> CitiesId { get; set; } = new();

        public IEnumerable<CheckboxOption> CheckboxCities { get; set; }


        public List<string> KeywordsId { get; set; } = new();

        public IEnumerable<CheckboxOption> CheckboxKeywords { get; set; }


        public List<string> SpheresId { get; set; } = new();

        public IEnumerable<CheckboxOption> CheckboxSpheres { get; set; }


        public List<string> SpecializationsId { get; set; } = new();

        public IEnumerable<CheckboxOption> CheckboxSpecializations { get; set; }


        public List<string> RemotenessesId { get; set; } = new();

        public IEnumerable<CheckboxOption> CheckboxRemotenesses { get; set; }


        public List<string> ExperienceLevelsId { get; set; } = new();

        public IEnumerable<CheckboxOption> CheckboxExperienceLevels { get; set; }


        public List<string> EnglishLevelsId { get; set; } = new();

        public IEnumerable<CheckboxOption> CheckboxEnglishLevels { get; set; }
    }
}
