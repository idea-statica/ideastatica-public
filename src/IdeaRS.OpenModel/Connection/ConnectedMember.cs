namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// ConnectedMember
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.SteelStructure.ConnectedMember,CI.StructuralElements", "CI.StructModel.Structure.IConnectedMember,CI.BasicTypes")]
	public class ConnectedMember : OpenElementId
	{
		/// <summary>
		/// MemberId
		/// </summary>
		public ReferenceElement MemberId { get; set; }

		/// <summary>
		/// IsContinuous
		/// </summary>
		public bool IsContinuous { get; set; }

		/// <summary>
		/// Get or set the identification in the original model
		/// In the case of the imported connection from another application
		/// </summary>
		public string OriginalModelId { get; set; }

		/// <summary>
		/// Length of the member
		/// </summary>
		public double Length { get; set; }
	}
}