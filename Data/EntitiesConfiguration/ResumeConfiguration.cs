using Data.Entities;
using Laraue.EfCoreTriggers.Common.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration
{
	internal class ResumeConfiguration : BaseEntityConfiguration<Resume>
	{
		public override void Configure(EntityTypeBuilder<Resume> builder)
		{
			base.Configure(builder);

			builder.AfterDelete(trigger => trigger
				.Action(action => action
					.Delete<VacancieRespond>((tableRefs, responds) => tableRefs.Old.Id == responds.ResumeId)
					.Delete<JobOffer>((tableRefs, offers) => tableRefs.Old.Id == offers.ResumeId)
				));
		}
	}
}
