using IdeaRS.OpenModel.Geometry2D;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaNegativePlate : AbstractIdeaObject<IIdeaNegativePlate>, IIdeaNegativePlate
	{
		public virtual IIdeaPersistenceToken Token { get; set; }

		public double Thickness { get; set; }

		public virtual IIdeaMaterial Material { get; set; }

		public virtual IIdeaNode Origin { get; set; }

		public CoordSystem LocalCoordinateSystem { get; set; }

		public Region2D Geometry { get; set; }

		protected IdeaNegativePlate(Identifier<IIdeaNegativePlate> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaNegativePlate(int id)
			: this(new IntIdentifier<IIdeaNegativePlate>(id))
		{ }

		public IdeaNegativePlate(string id)
			: this(new StringIdentifier<IIdeaNegativePlate>(id))
		{ }
	}
}
