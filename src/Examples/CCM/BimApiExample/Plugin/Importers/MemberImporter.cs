using BimApiExample.Plugin.BimApi;
using IdeaStatica.BimApiLink.BimApi;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace BimApiExample.Plugin.Importers
{
	internal class MemberImporter : IntIdentifierImporter<IIdeaMember1D>
	{
		public override IIdeaMember1D Create(int id)
		{
			return new Member1D(id)
			{
				Type = IdeaRS.OpenModel.Model.Member1DType.Beam,
				CrossSectionNo = 1,
				Elements = new List<IIdeaElement1D>() { new IdeaElement1D(id) { Segment = CreateSegment(id), }, },
			};
		}

		private IIdeaSegment3D CreateSegment(int id)
		{
			var (beg, end) = GetNodes(id);
			var lcs = GetCoordSystem(id);

			return new Segment3D(id)
			{
				StartNodeNo = beg,
				EndNodeNo = end,
				LocalCoordinateSystem = lcs
			};
		}

		private static (int beg, int end) GetNodes(int id)
		{
			if (id == 1)
			{
				return (1, 2);
			}

			return (2, 3);
		}

		private static IdeaRS.OpenModel.Geometry3D.CoordSystem GetCoordSystem(int id)
		{
			if (id == 1)
			{
				return new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
				{
					VecX = new IdeaRS.OpenModel.Geometry3D.Vector3D { Z = +1 },
					VecY = new IdeaRS.OpenModel.Geometry3D.Vector3D { Y = +1 },
					VecZ = new IdeaRS.OpenModel.Geometry3D.Vector3D { X = -1 },
				};
			}

			return new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
			{
				VecX = new IdeaRS.OpenModel.Geometry3D.Vector3D { X = 1 },
				VecY = new IdeaRS.OpenModel.Geometry3D.Vector3D { Y = 1 },
				VecZ = new IdeaRS.OpenModel.Geometry3D.Vector3D { Z = 1 },
			};
		}
	}
}