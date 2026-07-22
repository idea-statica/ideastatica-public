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

		/// <summary>
		/// True when this connected member was manually added or kept by the user in Checkbot
		/// (not from the original CAD/BIM import). The flag is preserved across CAD/FEA Sync so
		/// the user's choice survives re-import from the source model. Default false (CAD-imported).
		/// Introduced in IOM 3.3.0 (US 33733).
		/// </summary>
		public bool IsUserEdited { get; set; }
	}
}