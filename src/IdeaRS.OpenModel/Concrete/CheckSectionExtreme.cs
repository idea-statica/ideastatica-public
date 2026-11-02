using IdeaRS.OpenModel.Concrete.Load;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Check section extreme base class
	/// </summary>
	[XmlInclude(typeof(StandardCheckSectionExtreme))]
	[XmlInclude(typeof(StagedCheckSectionExtreme))]
	public abstract class CheckSectionExtreme
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected CheckSectionExtreme()
		{
			Description = string.Empty;
			Fundamental = null;
			Accidental = null;
			Characteristic = null;
			QuasiPermanent = null;
			Frequent = null;
			Fatigue = null;
		}

		/// <summary>
		/// Description of this extreme
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Fundamental loading
		/// </summary>
		public LoadingULS Fundamental { get; set; }

		/// <summary>
		/// Accidental loading
		/// </summary>
		public LoadingULS Accidental { get; set; }

		/// <summary>
		/// Characterictic loading
		/// </summary>
		public LoadingSLS Characteristic { get; set; }

		/// <summary>
		/// Quasi-Permanent loading
		/// </summary>
		public LoadingSLS QuasiPermanent { get; set; }

		/// <summary>
		/// Frequent loading
		/// </summary>
		public LoadingSLS Frequent { get; set; }

		/// <summary>
		/// Quasi-Permanent loading
		/// </summary>
		public FatigueLoading Fatigue { get; set; }
	}
}