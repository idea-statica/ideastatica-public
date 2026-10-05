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
		public string Name { get; set; }

		/// <summary>
		/// The description of the project
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Project number
		/// </summary>
		public string ProjectNumber { get; set; }

		/// <summary>
		/// Name of the author
		/// </summary>
		public string Author { get; set; }

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
		/// The edition of the steel design code for the project's connections (for the American code
		/// this is the LRFD/ASD choice).
		///
		/// On create it is applied to the connection the project starts with, so a project can be made
		/// as e.g. AISC 360-22 ASD in one call; an edition that does not belong to
		/// <see cref="CountryCode"/> is rejected with 422. It is reported back only when every
		/// connection shares one edition - the edition itself lives per connection, see
		/// <c>ConConnection.SteelEdition</c>.
		/// </summary>
		public ConSteelCodeEditionEnum SteelEdition { get; set; }

		/// <summary>
		/// Date
		/// </summary>
		public DateTime Date { get; set; }
	}
}
