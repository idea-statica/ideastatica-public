namespace IdeaStatiCa.BentleyCrossSections.CrossSectionConversions
{
#if NET48
	using System;
	using System.Text.RegularExpressions;

	public static class GroupCollectionExtensions
	{
		public static bool TryGetValue(this GroupCollection collection, string key, out Group value)
		{
			try
			{
				Group group = collection[key];
				if (group == null || group.Name == string.Empty)
				{
					value = null;
					return false;
				}

				value = group;
				return true;
			}
			catch
			{
				value = null;
				return false;
			}
		}
	}

	public static class StringExtensions
	{
		public static string[] Split(this string value, char separator, StringSplitOptions options)
		{
			return value.Split(new char[] { separator }, options);
		}
	}
#endif
}
