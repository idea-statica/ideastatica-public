
namespace IdeaStatiCa.BimApi
{
	public interface IIdeaWorkPlane : IIdeaObject
	{
		/// <summary>
		/// Workplane origin point 
		/// </summary>
		IIdeaNode Origin { get; }

		/// <summary>
		/// Workplane normal vector
		/// </summary>
		IdeaVector3D Normal { get; }
	}
}
