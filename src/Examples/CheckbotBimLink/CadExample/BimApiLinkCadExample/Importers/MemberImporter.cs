using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.CadExampleApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using MathNet.Spatial.Euclidean;
using System.Collections.Generic;
using BimApiLinkCadExample.Utils;

namespace BimApiLinkCadExample.Importers
{
	internal class MemberImporter : BaseImporter<IIdeaMember1D>
	{
		public MemberImporter(ICadGeometryApi model) : base(model)
		{
		}

		public override IIdeaMember1D Create(int id)
		{
			CadMember member = Model.GetMember(id);
			return new Member1D(id)
			{
				Type = IdeaRS.OpenModel.Model.Member1DType.Beam,
				CrossSectionNo = member.CrossSection,
				Elements = new List<IIdeaElement1D>() { new IdeaElement1D(id) { Segment = CreateSegment(member), }, },
			};
		}

		private IIdeaSegment3D CreateSegment(CadMember member)
		{
			var lcs = Model.GetMemberLcs(member.Id);

			return new Segment3D(member.Id)
			{
				StartNodeNo = PointTranslator.GetPointId(member.StartPoint),
				EndNodeNo = PointTranslator.GetPointId(member.EndPoint),
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