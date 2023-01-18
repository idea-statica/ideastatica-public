using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Parameters;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaBoltGrid : AbstractIdeaObject<IIdeaBoltGrid>, IIdeaBoltGrid
	{
		public IdeaBoltGrid(Identifier<IIdeaBoltGrid> identifier) : base(identifier)
		{
		}

		public IdeaBoltGrid(string id) : this(new StringIdentifier<IIdeaBoltGrid>(id))
		{ }

		public virtual IIdeaNode Origin { get; set; }

		public CoordSystem LocalCoordinateSystem { get; set; }

		public IEnumerable<IIdeaNode> Positions { get; set; }

		public IEnumerable<IIdeaObjectConnectable> ConnectedParts { get; set; }

		public bool ShearInThread { get; set; }

		public BoltShearType BoltShearType { get; set; }

		public IIdeaBoltAssembly BoltAssembly { get; set; }

		public IIdeaPersistenceToken Token { get; set; }
	}
}
