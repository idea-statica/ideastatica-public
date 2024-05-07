using System;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utils
{

	public static class StringExtensions
	{
		/// <summary>
		/// Check if string contains substring
		/// </summary>
		/// <param name="source"></param>
		/// <param name="toCheck"></param>
		/// <param name="comp"></param>
		/// <returns></returns>
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source?.IndexOf(toCheck, comp) >= 0;
		}
	}

}
