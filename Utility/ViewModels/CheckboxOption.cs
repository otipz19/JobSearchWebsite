namespace Utility.ViewModels
{
	public class CheckboxOption
	{
        public bool IsChecked { get; set; }

        public string Value { get; set; }

        public int Id => int.Parse(Value);

        public string Text { get; set; }
    }
}
