using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Type of loading
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public enum LoadingType
	{
		/// <summary>
		/// Load case
		/// </summary>
		LoadCase = 0,

		/// <summary>
		/// Combination
		/// </summary>
		Combination = 1,

		/// <summary>
		/// Result Class
		/// </summary>
		ResultClass = 2,
	}

	/// <summary>
	/// Loading identification
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	//[XmlInclude(typeof(ResultOfLoading2))]
	public class Loading
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Loading()
		{
			LoadingType = LoadingType.LoadCase;
			Id = 0;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="loadingType">Type of loading</param>
		/// <param name="id">Id of loading</param>
		public Loading(LoadingType loadingType, int id)
		{
			LoadingType = loadingType;
			Id = id;
		}

		/// <summary>
		/// Type of loading
		/// </summary>
		public LoadingType LoadingType { get; set; }

		/// <summary>
		/// Id of loading
		/// </summary>
		public int Id { get; set; }
	}
}