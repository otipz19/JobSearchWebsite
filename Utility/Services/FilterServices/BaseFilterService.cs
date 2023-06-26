using Data;
using Data.Entities;
using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Utility.Interfaces.Checkbox;

namespace Utility.Services.FilterServices
{
    public abstract class BaseFilterService<T>
		where T : BaseFilterableEntity
	{
		protected readonly AppDbContext _dbContext;
		protected readonly ICheckboxService _checkboxService;

		protected BaseFilterService(AppDbContext dbContext,
			ICheckboxService checkboxService)
		{
			_dbContext = dbContext;
			_checkboxService = checkboxService;
		}

		public virtual IQueryable<T> ApplyFilter(IQueryable<T> query, VacancieResumeFilter filter)
		{
			if (!filter.SearchQuery.IsNullOrEmpty())
			{
				query = query.Where(v => v.Name.Contains(filter.SearchQuery));
			}

			query.Include(v => v.Keywords);

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

		public async Task<VacancieResumeFilter> PopulateFilter(VacancieResumeFilter filter)
		{
			return SetIsChecked(await SetCheckboxes(filter));
		}

		private VacancieResumeFilter SetIsChecked(VacancieResumeFilter filter)
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

		private async Task<VacancieResumeFilter> SetCheckboxes(VacancieResumeFilter filter)
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
