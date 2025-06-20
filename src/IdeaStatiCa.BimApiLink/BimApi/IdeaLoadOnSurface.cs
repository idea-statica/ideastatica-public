using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaLoadOnSurface : AbstractIdeaObject<IIdeaLoadOnSurface>, IIdeaLoadOnSurface
	{
		/// <summary>
		/// Force in X direction
		/// </summary>
		public double Fx { get; set; }

		/// <summary>
		/// Force in Y direction
		/// </summary>
		public double Fy { get; set; }

		/// <summary>
		/// Force in Z direction
		/// </summary>
		public double Fz { get; set; }

		/// <summary>
		/// Load direction LCS or GCS
		/// </summary>
		public LoadDirection Direction { get; set; }

		/// <summary>
		/// Gets, sets geometry of this LoadOnSurface
		/// </summary>
		public Region3D Geometry { get; set; }

		/// <summary>
		/// Item where load is applied
		/// </summary>
		public IIdeaRegion3D ReferencedGeometry { get; set; }		

		public IdeaLoadOnSurface(Identifier<IIdeaLoadOnSurface> identifer)
			: base(identifer)
		{ }

		public IdeaLoadOnSurface(int id)
			: this(new IntIdentifier<IIdeaLoadOnSurface>(id))
		{ }

		public IdeaLoadOnSurface(string id)
			: this(new StringIdentifier<IIdeaLoadOnSurface>(id))
		{ }
	}
}
