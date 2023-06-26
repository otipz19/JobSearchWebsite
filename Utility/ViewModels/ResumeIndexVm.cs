using Data.Entities;

namespace Utility.ViewModels
{
    public class ResumeIndexVm
    {
        public Resume Resume { get; set; }

        public string ShortDescription { get; set; }

        public string CreatedAgo { get; set; }
    }
}
