using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Load group
	/// </summary>
	[XmlInclude(typeof(LoadGroupEC))]
	[XmlInclude(typeof(LoadGroupSIA))]
	public abstract class LoadGroup : OpenElementId
	{
		/// <summary>
		/// Name of load group
		/// </summary>
		public System.String Name { get; set; }

		/// <summary>
		/// Relation
		/// </summary>
		public Relation Relation { get; set; }

		/// <summary>
		/// Group type
		/// </summary>
		public LoadGroupType GroupType { get; set; }

		/// <summary>
		/// γ <sub>Q</sub>
		/// </summary>
		public System.Double GammaQ { get; set; }

		/// <summary>
		/// ζ
		/// </summary>
		public System.Double Dzeta { get; set; }

		/// <summary>
		/// γ <sub>G,Inf</sub>
		/// </summary>
		public System.Double GammaGInf { get; set; }

		/// <summary>
		/// γ <sub>G,Sup</sub>
		/// </summary>
		public System.Double GammaGSup { get; set; }
	}

	/// <summary>
	/// Load group type
	/// </summary>
	public enum LoadGroupType
	{
		/// <summary>
		/// Permanent
		/// </summary>
		Permanent = 0,

		/// <summary>
		/// Variable
		/// </summary>
		Variable = 1,

		/// <summary>
		/// Accidental
		/// </summary>
		Accidental = 2,

		/// <summary>
		/// Seismic
		/// </summary>
		Seismic = 3,

		/// <summary>
		/// Fatigue
		/// </summary>
		Fatigue = 4,

		///// <summary>
		///// for non-linear combination in load case
		///// </summary>
		//NonLinear = 5,
	}

	/// <summary>
	/// Relation
	/// </summary>
	public enum Relation
	{
		/// <summary>
		/// More load cases  from this load group will be used in a combination
		/// </summary>
		Standard,

		/// <summary>
		/// Only one load case from this load group will be used in a combination
		/// </summary>
		Exclusive
	}
}