using Data.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Utility.Interfaces.Checkbox;
using Utility.ViewModels;

namespace Utility.Services.Checkbox
{
    public class CheckboxService : ICheckboxService
	{
		public async Task<List<CheckboxOption>> MapFromEntities(IQueryable<BaseFilteringEntity> entities)
		{
			return await entities.Select(f => new CheckboxOption()
			{
				IsChecked = false,
				Text = f.Name,
				Value = f.Id.ToString(),
			})
			.ToListAsync();
		}

		public IEnumerable<CheckboxOption> SetIsChecked(IEnumerable<CheckboxOption> checkboxes, IEnumerable<int> selectedIds)
		{
			foreach (int id in selectedIds)
			{
				checkboxes.First(checkbox => checkbox.Id == id).IsChecked = true;
			}
			return checkboxes;
		}

		public IEnumerable<CheckboxOption> SetIsChecked(IEnumerable<CheckboxOption> checkboxes, IEnumerable<string> selectedIds)
		{
			return SetIsChecked(checkboxes, selectedIds.Select(int.Parse));
		}
	}
}
