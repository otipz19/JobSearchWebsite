using Data;
using Data.Entities;
using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Utility.Interfaces.Checkbox;
using Utility.Interfaces.FilterServices;
using Utility.Services.Checkbox;
using Utility.Utilities;

namespace Utility.Services.FilterServices
{
    public class VacancieFilterService : IVacancieFilterService
	{
		private readonly AppDbContext _dbContext;
		private readonly ICheckboxService _checkboxService;

		public VacancieFilterService(AppDbContext dbContext,
			ICheckboxService checkboxService)
		{
			_dbContext = dbContext;
			_checkboxService = checkboxService;
		}

		public IQueryable<Vacancie> ApplyFilter(VacancieFilter filter)
		{
			var query = _dbContext.Vacancies.AsQueryable();

			if (!filter.SearchQuery.IsNullOrEmpty())
			{
				query = query.Where(v => v.Name.Contains(filter.SearchQuery));
			}

			if (filter.SalaryFrom.HasValue)
			{
				query = query.Where(v => v.LeftSalaryFork >= filter.SalaryFrom);
			}

			if (filter.SalaryTo.HasValue)
			{
				query = query.Where(v => v.RightSalaryFork <= filter.SalaryTo);
			}

			query.Include(v => v.Cities).Include(v => v.States).Include(v => v.Keywords);

			if (filter.CitiesId.Any())
			{
				List<int> ids = filter.CitiesId.Select(int.Parse).ToList();
				//v.Cities.Count(c => ids.Contains(c.Id)) > 0 - the way to check intersection of sets,
				//so it can be translated to SQL by EF Core
				query = query.Where(v => v.Cities.Count(c => ids.Contains(c.Id)) > 0);
			}

			if (filter.StatesId.Any())
			{
				List<int> ids = filter.StatesId.Select(int.Parse).ToList();
				query = query.Where(v => v.States.Count(s => ids.Contains(s.Id)) > 0);
			}

			if (filter.KeywordsId.Any())
			{
				List<int> ids = filter.KeywordsId.Select(int.Parse).ToList();
				query = query.Where(v => v.Keywords.Count(k => ids.Contains(k.Id)) > 0);
			}

			if (filter.SpheresId.Any())
			{
				List<int> ids = filter.SpheresId.Select(int.Parse).ToList();
				query = query.Where(v => ids.Contains(v.SphereId));
			}

			if (filter.SpecializationsId.Any())
			{
				List<int> ids = filter.SpecializationsId.Select(int.Parse).ToList();
				query = query.Where(v => ids.Contains(v.SpecializationId));
			}

			if (filter.RemotenessesId.Any())
			{
				List<int> ids = filter.RemotenessesId.Select(int.Parse).ToList();
				query = query.Where(v => ids.Contains(v.RemotenessId));
			}

			if (filter.ExperienceLevelsId.Any())
			{
				List<int> ids = filter.ExperienceLevelsId.Select(int.Parse).ToList();
				query = query.Where(v => ids.Contains(v.ExperienceLevelId));
			}

			if (filter.EnglishLevelsId.Any())
			{
				List<int> ids = filter.EnglishLevelsId.Select(int.Parse).ToList();
				query = query.Where(v => ids.Contains(v.EnglishLevelId));
			}

			return query;
		}

		public async Task<VacancieFilter> PopulateFilter(VacancieFilter filter)
		{
			return SetIsChecked(await SetCheckboxes(filter));
		}

		private VacancieFilter SetIsChecked(VacancieFilter filter)
		{
			_checkboxService.SetIsChecked(filter.CheckboxKeywords, filter.KeywordsId);
			_checkboxService.SetIsChecked(filter.CheckboxStates, filter.StatesId);
			_checkboxService.SetIsChecked(filter.CheckboxCities, filter.CitiesId);
			_checkboxService.SetIsChecked(filter.CheckboxSpheres, filter.SpheresId);
			_checkboxService.SetIsChecked(filter.CheckboxSpecializations, filter.SpecializationsId);
			_checkboxService.SetIsChecked(filter.CheckboxExperienceLevels, filter.ExperienceLevelsId);
			_checkboxService.SetIsChecked(filter.CheckboxEnglishLevels, filter.EnglishLevelsId);
			_checkboxService.SetIsChecked(filter.CheckboxRemotenesses, filter.RemotenessesId);
			return filter;
		}

		private async Task<VacancieFilter> SetCheckboxes(VacancieFilter filter)
		{
			filter.CheckboxCities = await _checkboxService.MapFromEntities(_dbContext.Cities);
			filter.CheckboxStates = await _checkboxService.MapFromEntities(_dbContext.States);
			filter.CheckboxKeywords = await _checkboxService.MapFromEntities(_dbContext.Keywords);
			filter.CheckboxSpheres = await _checkboxService.MapFromEntities(_dbContext.Spheres);
			filter.CheckboxRemotenesses = await _checkboxService.MapFromEntities(_dbContext.Remotenesses);
			filter.CheckboxSpecializations = await _checkboxService.MapFromEntities(_dbContext.Specializations);
			filter.CheckboxExperienceLevels = await _checkboxService.MapFromEntities(_dbContext.ExperienceLevels);
			filter.CheckboxEnglishLevels = await _checkboxService.MapFromEntities(_dbContext.EnglishLevels);

			return filter;
		}
	}
}
