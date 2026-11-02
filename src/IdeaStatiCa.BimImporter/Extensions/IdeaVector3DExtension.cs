using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Extensions
{
	internal static class IdeaVector3DExtension
	{
		/// <summary>
		/// Convert vector in to IOM vector representation
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static IdeaRS.OpenModel.Geometry3D.Vector3D ToIOMVector(this IdeaVector3D vector)
		{
			return new IdeaRS.OpenModel.Geometry3D.Vector3D() { X = vector.X, Y = vector.Y, Z = vector.Z };
		}
	}
}
