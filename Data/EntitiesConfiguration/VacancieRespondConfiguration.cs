using Data.Entities;
using Laraue.EfCoreTriggers.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Data.EntitiesConfiguration
{
	public class VacancieRespondConfiguration : IEntityTypeConfiguration<VacancieRespond>
	{
		public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<VacancieRespond> builder)
		{
			builder.HasKey(x => new { x.ResumeId, x.VacancieId });

			builder.HasOne(x => x.Resume)
				.WithMany(z => z.VacancieResponds)
				.HasForeignKey(x => x.ResumeId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(x => x.Vacancie)
				.WithMany(z => z.VacancieResponds)
				.HasForeignKey(x => x.VacancieId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Property(x => x.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.AfterInsert(trigger => trigger
                .Action(action => action
                    .Update<Vacancie>(
                        (tableRefs, vacancie) => tableRefs.New.VacancieId == vacancie.Id,
                        (tableRefs, vacancie) => new Vacancie() { RespondsCount = vacancie.RespondsCount + 1 })
                ));

            builder.AfterDelete(trigger => trigger
                .Action(action => action
                    .Update<Vacancie>(
                        (tableRefs, vacancie) => tableRefs.Old.VacancieId == vacancie.Id,
                        (tableRefs, vacancie) => new Vacancie() { RespondsCount = vacancie.RespondsCount - 1 })
                ));
        }
	}
}
