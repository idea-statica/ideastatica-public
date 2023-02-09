namespace IdeaStatiCa.BimApi
{
	public interface IIdeaBend : IIdeaObject
	{
		/// <summary>
		/// first plate
		/// </summary>
		IIdeaPlate Plate1 { get; }

		/// <summary>
		/// second plate
		/// </summary>
		IIdeaPlate Plate2 { get; }

		/// <summary>
		/// Radius
		/// </summary>
		double Radius { get; }

		/// <summary>
		/// Line on side of bend boundary
		/// </summary>
		IIdeaLineSegment3D LineOnSideBoundary1 { get; }

		/// <summary>
		/// Line on other side of bend boundary
		/// </summary>

		IIdeaLineSegment3D LineOnSideBoundary2 { get; }

		/// <summary>
		/// End Face Normal
		/// </summary>
		IdeaVector3D EndFaceNormal { get; }
	}
}
