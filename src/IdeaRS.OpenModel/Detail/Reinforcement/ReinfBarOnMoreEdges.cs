namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representing reinforcement bar input on more edges in IDEA StatiCa Detail
	/// </summary>
	[OpenModelClass(typeof(Reinforcement))]
	public class ReinfBarOnMoreEdges : ReinforcementGroup
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReinfBarOnMoreEdges()
		{
			MasterEdges = new System.Collections.Generic.List<ReferenceElement>();
			Covers = new System.Collections.Generic.List<double>();
		}

		/// <summary>
		/// Master component of reinforcement
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Master Edges
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> MasterEdges { get; set; }

		/// <summary>
		/// Covers
		/// </summary>
		public System.Collections.Generic.List<double> Covers { get; set; }

		/// <summary>
		/// Bar is on whole length of first edge
		/// </summary>
		public bool WholeFirstEdge { get; set; }

		/// <summary>
		/// Length of bar from first intersection in the direction of edge
		/// </summary>
		public double LengthFromFirstIntersection { get; set; }

		/// <summary>
		///  Bar is on whole length of last edge
		/// </summary>
		public bool WholeLastEdge { get; set; }

		/// <summary>
		/// Length of bar from last intersection in the direction of edge
		/// </summary>
		public double LengthFromLastIntersection { get; set; }
	}
}
