﻿using Data.Entities.Base;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Data.Entities
{
    public class Vacancie : BaseFiltereableEntity
    {
        public int LeftSalaryFork { get; set; }

        public int RightSalaryFork { get; set; }

        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual List<State> States { get; set; } = new();

        public virtual List<City> Cities { get; set; } = new();

        public Vacancie Update(Vacancie source)
        {
            if(source  == null)
                throw new ArgumentNullException();
            base.Update(source);
            this.LeftSalaryFork = source.LeftSalaryFork;
            this.RightSalaryFork = source.RightSalaryFork;
            return this;
        }
    }
}
