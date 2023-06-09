﻿using Data.Enums;

namespace Data.Entities
{
	public class VacancieRespond
	{
        public int VacancieId { get; set; }

        public virtual Vacancie Vacancie { get; set; }

        public int ResumeId { get; set; }

        public virtual Resume Resume { get; set; }

        public DateTime CreatedAt { get; set; }

        public RespondStatus Status { get; set; }

        public DateTime? StatusChangedAt { get; set; }
    }
}
