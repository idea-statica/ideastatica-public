using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Reinforcement bar surface
	/// </summary>
	public enum ReinfBarSurface
	{
		/// <summary>
		/// Smooth bar surface
		/// </summary>
		Smooth,

		/// <summary>
		/// Ribbed bar surface
		/// </summary>
		Ribbed
	}

	/// <summary>
	/// Reinforcement stress-strain diagram
	/// </summary>
	public enum ReinfDiagramType
	{
		/// <summary>
		/// Billinear with an inclined top branch
		/// </summary>
		BilinerWithAnInclinedTopBranch,

		/// <summary>
		/// Billinear without an inclined top branch
		/// </summary>
		BilinerWithOutAnInclinedTopBranch,

		/// <summary>
		/// Defined by user
		/// </summary>
		DefinedByUser
	}

	/// <summary>
	/// Material reinforcement base class
	/// </summary>
	[XmlInclude(typeof(MatReinforcementEc2))]
	[XmlInclude(typeof(MatReinforcementACI))]
	public abstract class MatReinforcement : Material
	{
		/// <summary>
		/// Surface of bar
		/// </summary>
		public ReinfBarSurface BarSurface { get; set; }
	}
}