using Data.Entities.Base;

namespace JobSearchWebsite.Data.Entities
{
    public class City : BaseFilteringEntity
    {
        public virtual List<Vacancie> Vacancies { get; set; } = new();

    }
}
