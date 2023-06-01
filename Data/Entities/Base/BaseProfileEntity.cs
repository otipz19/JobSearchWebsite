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

		public string About { get; set; }

		public BaseProfileEntity Update(BaseProfileEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException();
			if(!entity.Name.IsNullOrEmpty())
                this.Name = entity.Name;
            if (!entity.About.IsNullOrEmpty())
                this.About = entity.About;
            if (!entity.ImagePath.IsNullOrEmpty())
                this.ImagePath = entity.ImagePath;
			return this;
		}
	}
}
