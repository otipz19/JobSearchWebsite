namespace Utility.Exceptions
{
	public class NoAccessException : ApplicationException
	{
		public NoAccessException()
		{
		}

		public NoAccessException(string message) : base(message)
		{
		}
	}
}
