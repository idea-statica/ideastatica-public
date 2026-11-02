using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Parameters;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaBoltGrid : AbstractIdeaObject<IIdeaBoltGrid>, IIdeaBoltGrid
	{
		protected IdeaBoltGrid(Identifier<IIdeaBoltGrid> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaBoltGrid(int id)
			: this(new IntIdentifier<IIdeaBoltGrid>(id))
		{ }

		public IdeaBoltGrid(string id)
			: this(new StringIdentifier<IIdeaBoltGrid>(id))
		{ }

		public virtual IIdeaNode Origin { get; set; }

		public CoordSystem LocalCoordinateSystem { get; set; }

		public IEnumerable<IIdeaNode> Positions { get; set; }

		public IEnumerable<IIdeaObjectConnectable> ConnectedParts { get; set; }

		public bool ShearInThread { get; set; }

		public BoltShearType BoltShearType { get; set; }

		public IIdeaBoltAssembly BoltAssembly { get; set; }

		public IIdeaPersistenceToken Token { get; set; }
		public double Length { get; set; }
	}
}
