﻿using Utility.Services.FilterServices;
using Utility.Services.OrderServices;

namespace Utility.ViewModels
{
    public class VacancieIndexListVm
	{
        public IEnumerable<VacancieIndexVm> Items { get; set; }

        public VacancieResumeFilter Filter { get; set; }

        public VacancieResumeOrder Order { get; set; }

        public int TotalCount { get; set; }

        public int? CurrentPage { get; set; }

        public bool IsPreviousDisabled { get; set; }

        public bool IsNextDisabled { get; set; }
    }
}
