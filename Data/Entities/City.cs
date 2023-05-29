using JobSearchWebsite.Data.Entities;
using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.Data.Entities
{
    public class City : BaseNamedEntity
    {
        public virtual List<Vacancie> Vacancies { get; set; } = new();

    }
}
