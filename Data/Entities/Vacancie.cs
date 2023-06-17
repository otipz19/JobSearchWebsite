using Data.Entities.Base;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Vacancie : BaseFilterableEntity
    {
        public int LeftSalaryFork { get; set; }

        public int RightSalaryFork { get; set; }

        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual List<State> States { get; set; } = new();

        public virtual List<City> Cities { get; set; } = new();
    }
}
