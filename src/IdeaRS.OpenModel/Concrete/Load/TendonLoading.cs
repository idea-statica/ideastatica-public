using System.Collections.Generic;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Tendon loading base class
	/// </summary>
	[XmlInclude(typeof(TendonStressLoading))]
	[XmlInclude(typeof(TendonLossesLoading))]
	public abstract class TendonLoading
	{
	}

	/// <summary>
	/// Tendon stress loading
	/// </summary>
	public class TendonStressLoading : TendonLoading
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public TendonStressLoading()
		{
			Stresses = new List<TendonTimeStressLoading>();
		}

		/// <summary>
		/// Tendon stress
		/// </summary>
		public List<TendonTimeStressLoading> Stresses { get; set; }
	}

	/// <summary>
	/// Tendon stress loading
	/// </summary>
	public class TendonTimeStressLoading
	{
		/// <summary>
		/// Time
		/// </summary>
		public double Time { get; set; }

		/// <summary>
		/// Tendon stress
		/// </summary>
		public double Stress { get; set; }
	}

	/// <summary>
	/// Tendon losses loading
	/// </summary>
	public class TendonLossesLoading : TendonLoading
	{
		/// <summary>
		/// Time
		/// </summary>
		public double Time { get; set; }
	}
}