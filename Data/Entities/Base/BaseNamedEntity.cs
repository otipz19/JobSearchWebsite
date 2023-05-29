using System.ComponentModel.DataAnnotations;

namespace JobSearchWebsite.Data.Entities.Base
{
    public abstract class BaseNamedEntity : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
