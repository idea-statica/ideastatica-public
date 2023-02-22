using IdeaRS.OpenModel.Geometry2D;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaPlate : AbstractIdeaObject<IIdeaPlate>, IIdeaPlate
	{
		public virtual IIdeaPersistenceToken Token { get; set; }

		public double Thickness { get; set; }

		public virtual IIdeaMaterial Material { get; set; }

		public virtual IIdeaNode Origin { get; set; }

		public CoordSystem LocalCoordinateSystem { get; set; }

		public Region2D Geometry { get; set; }

		protected IdeaPlate(Identifier<IIdeaPlate> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaPlate(int id)
			: this(new IntIdentifier<IIdeaPlate>(id))
		{ }

		public IdeaPlate(string id)
			: this(new StringIdentifier<IIdeaPlate>(id))
		{ }
	}
}
