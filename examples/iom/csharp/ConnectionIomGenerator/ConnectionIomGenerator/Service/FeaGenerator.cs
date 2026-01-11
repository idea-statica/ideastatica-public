using ConnectionIomGenerator.Fea;
using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Numerics;

namespace ConnectionIomGenerator.Service
{
	internal class FeaGenerator
	{
		static readonly int ConnectionPointId = 1;
		static readonly Vector3 Origin = new Vector3(0, 0, 0);
		static readonly Vector3 AxisX = new Vector3(1,0,0);
		static readonly Vector3 AxisY = new Vector3(0, 1, 0);
		static readonly Vector3 AxisZ = new Vector3(0, 0, 1);
		static readonly float MemberLength = 3f;

		public FeaGenerator() { }

		internal FeaModel Generate(ConnectionInput connectionInput)
		{
			var feaModel = new FeaModel();

			// only one material is expected
			feaModel.Materials.Add(1, new IdeaStatiCa.BimApiLink.BimApi.IdeaMaterialByName(1) {
				MaterialType = IdeaStatiCa.BimApi.MaterialType.Steel,
				Name = connectionInput.Material
			});

			// get all used cross-sections
			var uniqueCssNames = connectionInput.Members.Select(m => m.CrossSection).Distinct();
			Dictionary<string, int> cssMap = new Dictionary<string, int>();
			int cssId = 1;
			foreach (var uniqueCssName in uniqueCssNames)
			{
				var css = new IdeaCrossSectionByName(cssId)
				{
					Name = uniqueCssName,
					Material = feaModel.Materials[1]
				};

				feaModel.CrossSections.Add(cssId, css);
				cssMap.Add(uniqueCssName, cssId);
				cssId++;
			}

			int nodeId = ConnectionPointId;
			int segmentId = 1;
			int memberId = 1;
			int elementId = 1;

			// add main connection point
			var connectionNode = new IdeaNode(1)
			{
				Vector = new IdeaStatiCa.BimApi.IdeaVector3D(0,0,0)
			};
			feaModel.Nodes.Add(nodeId++, connectionNode);

			foreach(var memInput in connectionInput.Members)
			{

				// calculate direction vector
				Vector3 dir = AxisX;


				float pitchInRadians = memInput.Pitch * MathF.PI / 180f;
				Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(AxisY, pitchInRadians);

				Vector3 vecX = new Vector3(rotationMatrix.M11, rotationMatrix.M21, rotationMatrix.M31);
				Vector3 vecY = new Vector3(rotationMatrix.M12, rotationMatrix.M22, rotationMatrix.M32);
				Vector3 vecZ = new Vector3(rotationMatrix.M13, rotationMatrix.M23, rotationMatrix.M33);

				CoordSystemByVector lcs = new CoordSystemByVector()
				{
					VecX = new Vector3D(vecX.X, vecX.Y, vecX.Z),
					VecY = new Vector3D(vecY.X, vecY.Y, vecY.Z),
					VecZ = new Vector3D(vecZ.X, vecZ.Y, vecZ.Z)
				};

				dir = Vector3.Transform(dir, rotationMatrix);

				Vector3 memberOriginPt = Origin + dir * MemberLength;

				int beginPtId = nodeId++;
				// add continuous member
				var beginNode = new IdeaNode(beginPtId)
				{
					Vector = new IdeaStatiCa.BimApi.IdeaVector3D(memberOriginPt.X, memberOriginPt.Y, memberOriginPt.Z)
				};
				feaModel.Nodes.Add(beginPtId, beginNode);

				if (memInput.IsContinuous)
				{
					// add continuous member with two elements (segments)
					int endPtId = nodeId++;

					Vector3 memberEndPt = Origin - dir * MemberLength;
					// add ended member
					var endNode = new IdeaNode(endPtId)
					{
						Vector = new IdeaStatiCa.BimApi.IdeaVector3D(memberEndPt.X, memberEndPt.Y, memberEndPt.Z)
					};

					feaModel.Nodes.Add(endPtId, endNode);

					int currentMemberId = memberId++;
					int currentElementId1 = elementId++;
					int currentElementId2 = elementId++;
					int currentSegmentId1 = segmentId++;
					int currentSegmentId2 = segmentId++;

					// First segment: beginNode to connectionNode
					IdeaLineSegment3D lineSegment1 = new IdeaLineSegment3D(currentSegmentId1)
					{
						StartNode = beginNode,
						EndNode = connectionNode,
						LocalCoordinateSystem = lcs
					};

					feaModel.LineSegments.Add(currentSegmentId1, lineSegment1);

					// Second segment: connectionNode to endNode
					IdeaLineSegment3D lineSegment2 = new IdeaLineSegment3D(currentSegmentId2)
					{
						StartNode = connectionNode,
						EndNode = endNode,
						LocalCoordinateSystem = lcs
					};

					feaModel.LineSegments.Add(currentSegmentId2, lineSegment2);

					// First element
					IdeaElement1D memElement1D_1 = new IdeaElement1D(currentElementId1)
					{
						StartCrossSection = feaModel.CrossSections[cssMap[memInput.CrossSection]],
						EndCrossSection = feaModel.CrossSections[cssMap[memInput.CrossSection]],
						RotationRx = 0,
						Segment = lineSegment1
					};

					feaModel.Elements1D.Add(currentElementId1, memElement1D_1);

					// Second element
					IdeaElement1D memElement1D_2 = new IdeaElement1D(currentElementId2)
					{
						StartCrossSection = feaModel.CrossSections[cssMap[memInput.CrossSection]],
						EndCrossSection = feaModel.CrossSections[cssMap[memInput.CrossSection]],
						RotationRx = 0,
						Segment = lineSegment2
					};

					feaModel.Elements1D.Add(currentElementId2, memElement1D_2);

					// Continuous member with both elements
					IdeaMember1D mem = new IdeaMember1D(currentMemberId)
					{
						CrossSection = feaModel.CrossSections[cssMap[memInput.CrossSection]],
						Elements = new List<IdeaStatiCa.BimApi.IIdeaElement1D> { memElement1D_1, memElement1D_2 },
					};

					feaModel.Members1D.Add(currentMemberId, mem);
				}
				else
				{
					// add Ended member with one segment

					int currentMemberId = memberId++;
					int currentElementId = elementId++;
					int currentSegmentId = segmentId++;

					IdeaLineSegment3D lineSegment1 = new IdeaLineSegment3D(currentSegmentId)
					{
						StartNode = beginNode,
						EndNode = connectionNode,
						LocalCoordinateSystem = lcs
					};

					feaModel.LineSegments.Add(currentSegmentId, lineSegment1);

					IdeaElement1D memElement1D = new IdeaElement1D(currentElementId)
					{
						StartCrossSection = feaModel.CrossSections[cssMap[memInput.CrossSection]],
						EndCrossSection = feaModel.CrossSections[cssMap[memInput.CrossSection]],
						RotationRx = 0,
						Segment = lineSegment1
					};

					feaModel.Elements1D.Add(currentElementId, memElement1D);

					IdeaMember1D mem = new IdeaMember1D(currentMemberId)
					{
						CrossSection = feaModel.CrossSections[cssMap[memInput.CrossSection]],
						Elements = new List<IdeaStatiCa.BimApi.IIdeaElement1D> { memElement1D },
					};

					feaModel.Members1D.Add(currentMemberId, mem);
				}
			}

			return feaModel;
			
		}
	}
}
