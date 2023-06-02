using Data.Entities;

namespace JobSearchWebsite.MVC.ViewModels
{
    public class VacancieVM
    {
        public Vacancie Vacancie { get; set; }

        public IEnumerable<State> AvailableStates { get; set; }

        public IEnumerable<City> AvailableCitites { get; set; }

        public IEnumerable<Sphere> AvailableSpheres { get; set; }

        public IEnumerable<Specialization> AvailableSpecializations { get; set; }

        public IEnumerable<Keyword> AvailableKeywords { get; set; }

        public IEnumerable<Remoteness> AvailableRemotenesses { get; set; }

        public IEnumerable<ExperienceLevel> AvailableExperienceLevels { get; set; }

        public IEnumerable<EnglishLevel> AvailableEnglishLevels { get; set; }
    }
}
