using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.Conversion;
using CadExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Collections.Generic;
using Member1DType = IdeaRS.OpenModel.Model.Member1DType;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Imports a member with its straight axis.
	/// </summary>
	internal class MemberImporter : BaseImporter<IIdeaMember1D>
	{
		public MemberImporter(ICadGeometryApi cad)
			: base(cad)
		{
		}

		/// <summary>
		/// Converts the member with the given CAD id. Its axis runs between the nodes at its two ends.
		/// </summary>
		public override IIdeaMember1D Create(int id)
		{
			CadMember member = Cad.GetMember(id);
			var axis = new Segment3D(id)
			{
				StartNodeId = NodeId.Of(member.Start),
				EndNodeId = NodeId.Of(member.End),
				LocalCoordinateSystem = Cad.GetMemberCoordinateSystem(id).ToIdea(),
			};

			return new Member1D(id)
			{
				Type = Member1DType.Beam,
				CrossSectionName = member.CrossSectionName,
				Elements = new List<IIdeaElement1D> { new IdeaElement1D(id) { Segment = axis } },
			};
		}
	}
}
