namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Geotery interface
	/// </summary>
	public interface IGeometryPart
	{
		/// <summary>
		/// Lines of input edges on wall or beam
		/// </summary>
		System.Collections.Generic.List<ReferenceElement> Edges { get; set; }
	}
}