using IdeaRS.OpenModel;
using Newtonsoft.Json;
using System;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConProjectData
	{
		/// <summary>
		/// The name of the project
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// The description of the project
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Project number
		/// </summary>
		public string ProjectNumber { get; set; } = string.Empty;

		/// <summary>
		/// Name of the author
		/// </summary>
		public string Author { get; set; } = string.Empty;

		/// <summary>
		/// Design code of the project. Defaults to <see cref="CountryCode.ECEN"/>.
		/// Serialized as the <c>countryCode</c> field (by name).
		/// </summary>
		public CountryCode CountryCode { get; set; } = CountryCode.ECEN;

		/// <summary>
		/// Design code (legacy string accessor). Reads/writes <see cref="CountryCode"/>.
		/// </summary>
		[Obsolete("Use CountryCode (IdeaRS.OpenModel.CountryCode) instead. This string accessor will be removed in a future release.")]
		[JsonIgnore]
		public string DesignCode
		{
			get => CountryCode.ToString();
			set => CountryCode = Enum.TryParse<CountryCode>(value, ignoreCase: true, out var code) ? code : CountryCode.ECEN;
		}

		/// <summary>
		/// Date
		/// </summary>
		public DateTime Date { get; set; }
	}
}
