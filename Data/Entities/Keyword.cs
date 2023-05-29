using JobSearchWebsite.Data.Entities.Base;

namespace JobSearchWebsite.Data.Entities
{
    public class Keyword : BaseNamedEntity
    {
        public virtual List<Vacancie> Vacancies { get; set; } = new();
    }
}
