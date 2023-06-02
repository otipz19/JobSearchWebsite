using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Base
{
    public abstract class BaseNamedEntity : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public BaseNamedEntity Update(BaseNamedEntity source)
        {
            if (source == null)
                throw new ArgumentNullException();
            if (!source.Name.IsNullOrEmpty())
                this.Name = source.Name;
            return this;
        }
    }
}
