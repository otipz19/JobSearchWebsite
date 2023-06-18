using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.EntitiesConfiguration
{
	public class JobOfferConfiguration : IEntityTypeConfiguration<JobOffer>
	{
		public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<JobOffer> builder)
		{
			builder.HasKey(x => new { x.ResumeId, x.CompanyId });

			builder.HasOne(x => x.Resume)
				.WithMany(z => z.JobOffers)
				.HasForeignKey(x => x.ResumeId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(x => x.Company)
				.WithMany(z => z.JobOffers)
				.HasForeignKey(x => x.CompanyId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(x => x.Vacancie)
				.WithMany(z => z.JobOffers)
				.HasForeignKey(x => x.VacancieId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Property(x => x.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP");
		}
	}
}
