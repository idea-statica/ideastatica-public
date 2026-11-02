using System.Collections.Generic;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Results for group of structural elements
	/// </summary>
	/// <typeparam name="T">Type of result</typeparam>
	public class ResultsForSelection<T>
	{
		public List<T> Results { get; set; }
	}
}
