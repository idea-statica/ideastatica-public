using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaElement2D : AbstractIdeaObject<IIdeaElement2D>, IIdeaElement2D
	{
		public virtual IIdeaMaterial Material { get; set; }

		public virtual IIdeaRegion3D GeometricRegion { get; set; }

		public virtual double Thickness { get; set; }

		public virtual double EccentricityZ { get; set; }

		public virtual List<IIdeaPolyLine3D> InnerLines { get; set; }

		public virtual List<IIdeaNode> InnerPoints { get; set; }

		public virtual Element2DType ElementType { get; set; }

		public IdeaElement2D(Identifier<IIdeaElement2D> identifer)
	:		base(identifer)
		{ }

		public IdeaElement2D(int id)
			: this(new IntIdentifier<IIdeaElement2D>(id))
		{ }

		public IdeaElement2D(string id)
			: this(new StringIdentifier<IIdeaElement2D>(id))
		{ }
	}
}
