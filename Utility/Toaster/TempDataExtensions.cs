using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace Utility.Toaster
{
    public static class TempDataExtensions
	{
		public static Toaster Toaster(this ITempDataDictionary tempData)
		{
			return new Toaster(tempData);
		}

		public static void Set<T>(this ITempDataDictionary tempData, string key, T value)
		{
			if (string.IsNullOrEmpty(key) || value == null)
				throw new ApplicationException();
			tempData[key] = JsonSerializer.Serialize(value);
		}

		public static T Get<T>(this ITempDataDictionary tempData, string key)
		{
			if (string.IsNullOrEmpty(key))
				throw new ApplicationException();
			object value;
			tempData.TryGetValue(key, out value);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value.ToString());
		}
	}
}
