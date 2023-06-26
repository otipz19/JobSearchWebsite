using Utility.Services.Checkbox;

namespace Utility.ViewModels
{
    public class IndexCheckboxPartialVm
    {
        public IndexCheckboxPartialVm(IEnumerable<CheckboxOption> checkboxOptions, string label, string inputName)
        {
            CheckboxOptions = checkboxOptions;
            Label = label;
            InputName = inputName;
        }

        public IEnumerable<CheckboxOption> CheckboxOptions { get; set; }

        public string Label { get; set; }

        public string InputName { get; set; }

        public string AccordionId { get; set; }
    }
}
