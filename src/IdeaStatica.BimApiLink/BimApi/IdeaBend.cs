using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaBend : AbstractIdeaObject<IIdeaBend>, IIdeaBend
	{
		//public virtual IIdeaPersistenceToken Token { get; set; }

		public IIdeaPlate Plate1 { get; set; }

		public IIdeaPlate Plate2 { get; set; }

		public double Radius { get; set; }

		public IIdeaLineSegment3D LineOnSideBoundary1 { get; set; }

		public IIdeaLineSegment3D LineOnSideBoundary2 { get; set; }

		public IdeaVector3D EndFaceNormal { get; set; }

		protected IdeaBend(Identifier<IIdeaBend> identifer)
		: base(identifer)
		{
			//Token = identifer;
		}

		public IdeaBend(int id)
			: this(new IntIdentifier<IIdeaBend>(id))
		{ }

		public IdeaBend(string id)
			: this(new StringIdentifier<IIdeaBend>(id))
		{ }
	}
}
