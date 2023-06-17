using Data.Entities.Base;

namespace Data.Entities
{
	public class Resume : BaseFilterableEntity
	{
        public string DocumentPath { get; set; }

        public int WantedSalary { get; set; }

        public int JobseekerId { get; set; }

        public virtual Jobseeker Jobseeker { get; set; }

        public int StateId { get; set; }

        public virtual State State { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }
	}
}
