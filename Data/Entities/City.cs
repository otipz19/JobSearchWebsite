using Data.Entities.Base;

namespace Data.Entities
{
    public class City : BaseFilteringEntity
    {
        public virtual List<Vacancie> Vacancies { get; set; } = new();

    }
}
