namespace Data.Entities
{
	public class JobOffer
	{
		public int ResumeId { get; set; }

		public virtual Resume Resume { get; set; }

		public int CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? VacancieId { get; set; }

		public virtual Vacancie Vacancie { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }
	}
}
