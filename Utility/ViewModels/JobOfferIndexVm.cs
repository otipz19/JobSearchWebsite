using Data.Entities;

namespace Utility.ViewModels
{
	public class JobOfferIndexVm
	{
        public JobOffer JobOffer { get; set; }

        public string SentAgo { get; set; }

        public string AnsweredAgo { get; set; }
    }
}
