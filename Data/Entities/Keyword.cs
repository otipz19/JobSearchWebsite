using Data.Entities.Base;
using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.Data.Entities
{
    public class Keyword : BaseFilteringEntity
	{
        public virtual List<Vacancie> Vacancies { get; set; } = new();
    }
}
