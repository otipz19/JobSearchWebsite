using Data.Entities;

namespace Utility.ViewModels
{
    public class JobseekerDetailsVm
    {
        public Jobseeker Jobseeker { get; set; }

        public IEnumerable<ResumeIndexVm> Resumes { get; set; }
    }
}
