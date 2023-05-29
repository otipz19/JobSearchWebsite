using Data.Entities.Base;

namespace Data.Entities
{
	public class Jobseeker : BaseProfileEntity
	{
        public virtual List<Resume> Resumes { get; set; }
    }
}
