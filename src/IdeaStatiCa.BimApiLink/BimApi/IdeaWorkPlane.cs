using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaWorkPlane : AbstractIdeaObject<IIdeaWorkPlane>, IIdeaWorkPlane
	{
		public virtual IIdeaPersistenceToken Token { get; set; }


		public virtual IIdeaNode Origin { get; set; }

		public IdeaVector3D Normal { get; set; }

		protected IdeaWorkPlane(Identifier<IIdeaWorkPlane> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaWorkPlane(int id)
			: this(new IntIdentifier<IIdeaWorkPlane>(id))
		{ }

		public IdeaWorkPlane(string id)
			: this(new StringIdentifier<IIdeaWorkPlane>(id))
		{ }

	}
}
