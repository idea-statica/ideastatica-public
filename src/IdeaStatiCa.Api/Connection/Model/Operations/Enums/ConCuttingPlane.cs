namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Which side of the cutting object is kept when more than one cut plane is possible.
	/// </summary>
	public enum ConCuttingPlane
	{
		/// <summary>Keep the part on the side closer to the cutting object's reference point.</summary>
		Closer,

		/// <summary>Keep the part on the side farther from the cutting object's reference point.</summary>
		Farther,
	}
}
