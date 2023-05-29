using Data.Entities.Base;

namespace Data.Entities
{
    public class Keyword : BaseFilteringEntity
	{
        public virtual List<Vacancie> Vacancies { get; set; } = new();

		public virtual List<Resume> Resumes { get; set; } = new();
	}
}
