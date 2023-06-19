namespace Utility.Utilities
{
	public static class DateTimeExtentions
	{
        /// <returns>
		/// Empty string if time passed less than 1 minute.
		/// Time representation in a whole measure otherwise
		/// </returns>
        public static string GetTimePassedString(this DateTime time)
		{
			TimeSpan timePassed = DateTime.Now - time;
			if (timePassed.Days > 0)
				return $" {timePassed.Days} days ago";
			else if (timePassed.Hours > 0)
				return $" {timePassed.Hours} hours ago";
			else if (timePassed.Minutes > 0)
				return $" {timePassed.Minutes} mins ago";
			return "";
		}

        /// <returns>
		/// Empty string if time is null or time passed less than 1 minute.
        /// Time representation in a whole measure otherwise
		/// </returns>
        public static string GetTimePassedString(this DateTime? time)
		{
			if (time == null)
				return "";
			return time.Value.GetTimePassedString();
		}
	}
}
