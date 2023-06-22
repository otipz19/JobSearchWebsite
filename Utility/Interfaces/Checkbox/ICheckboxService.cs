using Data.Entities.Base;
using Utility.Services.Checkbox;

namespace Utility.Interfaces.Checkbox
{
	public interface ICheckboxService
	{
		public Task<List<CheckboxOption>> MapFromEntities(IQueryable<BaseFilteringEntity> entities);

		public IEnumerable<CheckboxOption> SetIsChecked(IEnumerable<CheckboxOption> checkboxes, IEnumerable<int> selectedIds);

		public IEnumerable<CheckboxOption> SetIsChecked(IEnumerable<CheckboxOption> checkboxes, IEnumerable<string> selectedIds);
	}
}
