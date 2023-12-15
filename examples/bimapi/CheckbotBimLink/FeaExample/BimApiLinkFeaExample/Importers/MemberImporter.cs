using BimApiLinkFeaExample.BimApi;
using BimApiLinkFeaExample.FeaExampleApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using MathNet.Spatial.Euclidean;
using System.Collections.Generic;

namespace BimApiLinkFeaExample.Importers
{
	internal class MemberImporter : IntIdentifierImporter<IIdeaMember1D>
	{
		private readonly IFeaGeometryApi geometry;

		public MemberImporter(IFeaGeometryApi geometry)
		{
			this.geometry = geometry;
		}

		public override IIdeaMember1D Create(int id)
		{
			IFeaMember member = geometry.GetMember(id);
			return new Member1D(id)
			{
				Type = IdeaRS.OpenModel.Model.Member1DType.Beam,
				CrossSectionNo = member.CrossSectionId,
				Elements = new List<IIdeaElement1D>() { new IdeaElement1D(id) { Segment = CreateSegment(member), }, },
			};
		}

		private IIdeaSegment3D CreateSegment(IFeaMember member)
		{
			var lcs = geometry.GetMemberLcs(member.Id);

			return new Segment3D(member.Id)
			{
				StartNodeNo = member.BeginNode,
				EndNodeNo = member.EndNode,
				LocalCoordinateSystem = new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
				{
					VecX = ConvertVector(lcs.X),
					VecY = ConvertVector(lcs.Y),
					VecZ = ConvertVector(lcs.Z),
				},
			};
		}

		private static IdeaRS.OpenModel.Geometry3D.Vector3D ConvertVector(UnitVector3D v)
		{
			return new IdeaRS.OpenModel.Geometry3D.Vector3D
			{
				X = v.X,
				Y = v.Y,
				Z = v.Z,
			};
		}
	}
}