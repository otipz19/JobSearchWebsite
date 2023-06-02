using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Base
{
	public abstract class BaseFiltereableEntity : BaseNamedEntity
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

		public BaseFiltereableEntity Update(BaseFiltereableEntity source)
		{
			if(source == null) 
				throw new ArgumentNullException();
			base.Update(source);
			if(source.Description != null)
				this.Description = source.Description;
			return this;
		}
	}
}
