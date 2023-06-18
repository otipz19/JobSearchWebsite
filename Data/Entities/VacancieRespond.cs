namespace Data.Entities
{
	/// <summary>
	/// Represents respond to vacancie
	/// </summary>
	public class VacancieRespond
	{
        public int VacancieId { get; set; }

        public virtual Vacancie Vacancie { get; set; }

        public int ResumeId { get; set; }

        public virtual Resume Resume { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
