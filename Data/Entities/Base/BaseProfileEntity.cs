using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Base
{
	public abstract class BaseProfileEntity : BaseNamedEntity
	{
		[Required]
        public string AppUserId { get; set; }

		public virtual AppUser AppUser { get; set; }

        public string ImagePath { get; set; }

		[DataType(DataType.MultilineText)]
		public string About { get; set; }

		public BaseProfileEntity Update(BaseProfileEntity source)
		{
			if (source == null)
				throw new ArgumentNullException();
			base.Update(source);
            if (!source.About.IsNullOrEmpty())
                this.About = source.About;
            if (!source.ImagePath.IsNullOrEmpty())
                this.ImagePath = source.ImagePath;
			return this;
		}
	}
}
