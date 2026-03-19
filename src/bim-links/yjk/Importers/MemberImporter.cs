using yjk.BimApis;
using yjk.FeaApis;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using MathNet.Spatial.Euclidean;
using System.Collections.Generic;

namespace yjk.Importers
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

/*			IdeaRS.OpenModel.Geometry3D.CoordSystemByVector localCoordinateSystem = new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector() { };

			if (member.Type == MemberType.Column)
			{
				localCoordinateSystem.VecX = ConvertVector(lcs.Y);
				localCoordinateSystem.VecY = ConvertVector(lcs.X);
				localCoordinateSystem.VecZ = ConvertVector(lcs.Z);
			}
			else
			{
				localCoordinateSystem.VecX = ConvertVector(lcs.X);
				localCoordinateSystem.VecY = ConvertVector(lcs.Y);
				localCoordinateSystem.VecZ = ConvertVector(lcs.Z);
			}*/

			return new Segment3D(member.Id)
			{
				StartNodeNo = member.BeginNodeId,
				EndNodeNo = member.EndNodeId,
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