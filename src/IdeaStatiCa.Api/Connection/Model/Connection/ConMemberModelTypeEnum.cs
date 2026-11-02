namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// Analysis model type of a member - which force/moment components are transferred.
	/// Names and values mirror the internal BeamSegmentModelType so the two convert by name.
	/// </summary>
	public enum ConMemberModelTypeEnum
	{
		/// <summary>N-Vy-Vz-Mx-My-Mz</summary>
		LoadedInXYZ = 0,
		/// <summary>N-Vz-My</summary>
		LoadedInXZ = 2,
		/// <summary>N-Vy-Mz</summary>
		LoadedInXY = 4,
		/// <summary>N-Vy-Vz</summary>
		LoadedInXYZ_NoBending = 32,
	}
}
