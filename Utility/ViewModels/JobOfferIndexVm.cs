using Data.Entities;

namespace Utility.ViewModels
{
    public class JobOfferIndexVm
    {
        public List<JobOfferDetailsVm> JobOffers { get; set; }

        public Resume CommonResume { get; set; }

        public Vacancie CommonVacancie { get; set; }
    }
}
