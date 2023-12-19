namespace IdeaStatica.TeklaStructuresPlugin.Utils
{
	internal static class VectorExtension
	{
		/// <summary>
		/// Convert UnitVector3D in to OpenModel.Geometry3D.Vector3D
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static IdeaRS.OpenModel.Geometry3D.Vector3D ToIOM(this MathNet.Spatial.Euclidean.UnitVector3D vector)
		{
			return new IdeaRS.OpenModel.Geometry3D.Vector3D()
			{
				X = vector.X,
				Y = vector.Y,
				Z = vector.Z
			};
		}

		/// <summary>
		/// Convert UnitVector3D in to OpenModel.Geometry3D.Vector3D
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static MathNet.Spatial.Euclidean.UnitVector3D FromIOM(this IdeaRS.OpenModel.Geometry3D.Vector3D vector)
		{
			return MathNet.Spatial.Euclidean.UnitVector3D.Create(
					vector.X,
					vector.Y,
					vector.Z);
		}
	}
}