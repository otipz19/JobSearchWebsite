using Data.Entities;
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
		}
	}
}
