using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaFoldedPlate : IIdeaObjectConnectable
	{
		/// <summary>
		/// Collection of plates
		/// </summary>
		IEnumerable<IIdeaPlate> Plates { get; }

		/// <summary>
		/// Collection of plates
		/// </summary>
		IEnumerable<IIdeaBend> Bends { get; }
	}

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
		/// Line on otherside of bend boundary
		/// </summary>

		IIdeaLineSegment3D LineOnSideBoundary2 { get; }

		/// <summary>
		/// End Face Normal
		/// </summary>
		IdeaVector3D EndFaceNormal { get; }
	}
}
