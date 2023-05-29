using JobSearchWebsite.Data.Entities;
using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.Data.Entities
{
    public class State : BaseNamedEntity
    {
        public virtual List<Vacancie> Vacancies { get; set; } = new();

    }
}
