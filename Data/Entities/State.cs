using Data.Entities.Base;

namespace Data.Entities
{
    public class State : BaseFilteringEntity
	{
        public virtual List<Vacancie> Vacancies { get; set; } = new();

    }
}
