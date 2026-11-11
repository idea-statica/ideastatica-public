using BimApiLinkFeaExample.BimApi;
using FeaExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using System.Collections.Generic;
using CoordSystemByVector = IdeaRS.OpenModel.Geometry3D.CoordSystemByVector;
using Member1DType = IdeaRS.OpenModel.Model.Member1DType;
using Vector3D = IdeaRS.OpenModel.Geometry3D.Vector3D;

namespace BimApiLinkFeaExample.Importers
{
	/// <summary>
	/// Imports a member with its straight axis.
	/// </summary>
	internal class MemberImporter : IntIdentifierImporter<IIdeaMember1D>
	{
		private readonly IFeaGeometryApi _geometry;

		public MemberImporter(IFeaGeometryApi geometry)
		{
			_geometry = geometry;
		}

		/// <summary>
		/// Converts the member with the given FEA id. Its axis runs between its two end nodes, and its own axes are the
		/// ones its internal forces are given in.
		/// </summary>
		public override IIdeaMember1D Create(int id)
		{
			FeaMember member = _geometry.GetMember(id);
			FeaAxes axes = _geometry.GetMemberAxes(id);

			var axis = new Segment3D(id)
			{
				StartNodeId = member.StartNodeId,
				EndNodeId = member.EndNodeId,
				LocalCoordinateSystem = new CoordSystemByVector
				{
					VecX = ToIdea(axes.X),
					VecY = ToIdea(axes.Y),
					VecZ = ToIdea(axes.Z),
				},
			};

			return new Member1D(id)
			{
				Type = Member1DType.Beam,
				CrossSectionId = member.CrossSectionId,
				Elements = new List<IIdeaElement1D> { new IdeaElement1D(id) { Segment = axis } },
			};
		}

		private static Vector3D ToIdea(FeaVector3D vector) => new Vector3D { X = vector.X, Y = vector.Y, Z = vector.Z };
	}
}
