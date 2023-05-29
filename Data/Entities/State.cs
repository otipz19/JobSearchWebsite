using Data.Entities.Base;
using JobSearchWebsite.Data.Entities;
using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.Data.Entities
{
    public class State : BaseFilteringEntity
	{
        public virtual List<Vacancie> Vacancies { get; set; } = new();

    }
}
