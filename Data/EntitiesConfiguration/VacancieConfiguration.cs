using Data.Entities;
using Laraue.EfCoreTriggers.Common.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration
{
    internal class VacancieConfiguration : BaseEntityConfiguration<Vacancie>
    {
		public override void Configure(EntityTypeBuilder<Vacancie> builder)
		{
			base.Configure(builder);

			builder.AfterDelete(trigger => trigger
				.Action(action => action
					.Delete<VacancieRespond>((tableRefs, responds) => tableRefs.Old.Id == responds.VacancieId)
					.Update<JobOffer>(
						(tableRefs, offers) => tableRefs.Old.Id == offers.VacancieId,
						(tableRefs, offers) => new JobOffer() { VacancieId = null })
				));
		}
	}
}
