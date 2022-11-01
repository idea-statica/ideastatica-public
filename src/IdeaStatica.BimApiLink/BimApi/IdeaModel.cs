using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.BimApi
{
	public abstract class IdeaModel : BimLinkObject, IIdeaModel
	{
		public virtual ISet<IIdeaLoading> GetLoads()
		{
			return new HashSet<IIdeaLoading>();
		}

		public virtual ISet<IIdeaMember1D> GetMembers()
		{
			return new HashSet<IIdeaMember1D>();
		}

		public abstract OriginSettings GetOriginSettings();

		public abstract void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members);

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out ISet<IIdeaConnectionPoint> connectionPoints)
		{
			throw new NotImplementedException();
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out IIdeaConnectionPoint connectionPoint)
		{
			throw new NotImplementedException();
		}
	}
}