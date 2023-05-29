using System.ComponentModel.DataAnnotations;
using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.Data.Entities
{
    public class Vacancie : BaseNamedEntity
    {
        public int LeftSalaryFork { get; set; }

        public int RightSalaryFork { get; set; }

        [Required]
        public string Description { get; set; }

        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int SphereId { get; set; }

        public virtual Sphere Sphere { get; set; }

        public int SpecializationId { get; set; }

        public virtual Specialization Specialization { get; set; }

        public virtual List<Keyword> Keywords { get; set; } = new();

        public int RemotenessId { get; set; }

        public virtual Remoteness Remoteness { get; set; }

        public virtual List<State> States { get; set; } = new();

        public virtual List<City> Cities { get; set; } = new();

        public int ExperienceLevelId { get; set; }

        public virtual ExperienceLevel ExperienceLevel { get; set; }

        public int EnglishLevelId { get; set; }

        public virtual EnglishLevel EnglishLevel { get; set; }
    }
}
