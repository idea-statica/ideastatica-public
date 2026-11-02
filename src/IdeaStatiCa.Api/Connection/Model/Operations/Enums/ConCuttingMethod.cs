namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// How the cutting object trims the cut member.
	/// </summary>
	public enum ConCuttingMethod
	{
		/// <summary>Cut by the cutting object's bounding box.</summary>
		BoundingBox,

		/// <summary>Cut by the cutting object's surface.</summary>
		Surface,

		/// <summary>Cut by the cutting object's surface, all around.</summary>
		SurfaceAllAround,

		/// <summary>Mitre cut against the cutting object.</summary>
		MitreCut,
	}
}
