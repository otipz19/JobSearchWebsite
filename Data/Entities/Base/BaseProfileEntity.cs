using Data.Entities.Base;

namespace Data.Entities.Base
{
	public abstract class BaseProfileEntity : BaseNamedEntity
	{
		public string ImagePath { get; set; }

		public string About { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }
	}
}
