﻿using Data.Entities;
using Utility.Services.Checkbox;

namespace Utility.ViewModels
{
    public class ResumeUpsertVm
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int WantedSalary { get; set; }

        public int SphereId { get; set; }

        public int SpecializationId { get; set; }

        public int RemotenessId { get; set; }

        public int ExperienceLevelId { get; set; }

        public int EnglishLevelId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        public IEnumerable<Sphere> AvailableSpheres { get; set; }

        public IEnumerable<Specialization> AvailableSpecializations { get; set; }

        public IEnumerable<Remoteness> AvailableRemotenesses { get; set; }

        public IEnumerable<ExperienceLevel> AvailableExperienceLevels { get; set; }

        public IEnumerable<EnglishLevel> AvailableEnglishLevels { get; set; }

        public IEnumerable<State> AvailableStates { get; set; }

        public IEnumerable<City> AvailableCities { get; set; }

        public List<CheckboxOption> CheckboxKeywords { get; set; } = new();

        public List<string> SelectedKeywords { get; set; } = new();
    }
}
