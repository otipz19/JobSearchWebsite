using Data.Entities.Base;

namespace Data.Entities
{
    public class Company : BaseProfileEntity
    {
        public virtual List<Vacancie> Vacancies { get; set; }

        public virtual List<JobOffer> JobOffers { get; set; } = new();
    }
}
