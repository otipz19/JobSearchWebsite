﻿using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Base
{
	public abstract class BaseFilterableEntity : BaseNamedEntity
	{
        [Required]
		public string Description { get; set; }

		public int SphereId { get; set; }

		public virtual Sphere Sphere { get; set; }

		public int SpecializationId { get; set; }

		public virtual Specialization Specialization { get; set; }

		public virtual List<Keyword> Keywords { get; set; } = new();

		public int RemotenessId { get; set; }

		public virtual Remoteness Remoteness { get; set; }

		public int ExperienceLevelId { get; set; }

		public virtual ExperienceLevel ExperienceLevel { get; set; }

		public int EnglishLevelId { get; set; }

		public virtual EnglishLevel EnglishLevel { get; set; }

		public virtual List<VacancieRespond> VacancieResponds { get; set; } = new();

		public virtual List<JobOffer> JobOffers { get; set; } = new();


        public bool IsPublished { get; set; }

        public DateTime? PublishedAt { get; set; }

        public int CountWatched { get; set; }
    }
}
