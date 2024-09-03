using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Parameters;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaAnchorGrid : AbstractIdeaObject<IIdeaAnchorGrid>, IIdeaAnchorGrid
	{
		protected IdeaAnchorGrid(Identifier<IIdeaAnchorGrid> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaAnchorGrid(int id)
			: this(new IntIdentifier<IIdeaAnchorGrid>(id))
		{ }

		public IdeaAnchorGrid(string id)
			: this(new StringIdentifier<IIdeaAnchorGrid>(id))
		{ }

		public AnchorType AnchorType { get; set; }

		public IIdeaConcreteBlock ConcreteBlock { get; set; }

		public double WasherSize { get; set; }

		public virtual IIdeaNode Origin { get; set; }

		public CoordSystem LocalCoordinateSystem { get; set; }

		public IEnumerable<IIdeaNode> Positions { get; set; }

		public IEnumerable<IIdeaObjectConnectable> ConnectedParts { get; set; }

		public bool ShearInThread { get; set; }

		public BoltShearType BoltShearType { get; set; }

		public IIdeaBoltAssembly BoltAssembly { get; set; }

		public IIdeaPersistenceToken Token { get; set; }
		public double AnchoringLength { get; set; }
		public double HookLength { get; set; }
		public double Length { get; set; }
	}
}
