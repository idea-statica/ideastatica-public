namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Row-shift pattern for orthogonal fastener grids. Default is <see cref="Regular"/> (all rows aligned).
	/// </summary>
	public enum ConGridLayout
	{
		Regular,
		FirstShiftedFull,
		SecondShiftedFull,
		FirstShiftedShort,
		SecondShiftedShort,
	}
}
