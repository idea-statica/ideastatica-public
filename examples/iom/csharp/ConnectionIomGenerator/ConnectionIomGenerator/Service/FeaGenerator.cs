using ConnectionIomGenerator.Fea;
using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Numerics;

namespace ConnectionIomGenerator.Service
{
	public class FeaGenerator
	{
		static readonly int ConnectionPointId = 1;
		static readonly Vector3 Origin = new Vector3(0, 0, 0);
		static readonly Vector3 AxisX = new Vector3(1,0,0);
		static readonly Vector3 AxisY = new Vector3(0, 1, 0);
		static readonly Vector3 AxisZ = new Vector3(0, 0, 1);
		static readonly float MemberLength = 3f;

		public FeaGenerator() { }

		public FeaModel Generate(ConnectionInput connectionInput, LoadingInput? loadingInput)
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
				// Calculate direction vector
				Vector3 dir = AxisX;

				// Rotation around Y axis (pitch)
				float pitchInRadians = memInput.Pitch * MathF.PI / 180f;
				Matrix4x4 rotationAroundY = Matrix4x4.CreateFromAxisAngle(AxisY, pitchInRadians);

				// Rotation around Z axis (from Rotation property)
				float rotationInRadians = memInput.Direction * MathF.PI / 180f;
				Matrix4x4 rotationAroundZ = Matrix4x4.CreateFromAxisAngle(AxisZ, rotationInRadians);

				// Combine rotations (apply pitch first, then rotation around Z)
				Matrix4x4 rotationMatrix = rotationAroundY * rotationAroundZ;

				// Extract the coordinate system vectors
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
						StartNode = connectionNode,
						EndNode = beginNode,
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

			// Create connection point at origin with all members
			CreateConnectionPoint(feaModel, connectionInput);

			if (loadingInput != null)
			{
				// add load cases
				foreach (var lc in loadingInput.LoadCases)
				{
					var ideaLoading = new IdeaLoadCase(lc.Id) { Name = lc.Name };
					feaModel.Loading.Add(lc.Id, ideaLoading);
				}
			}

			return feaModel;
		}

		public LoadingInput CreateLoadingForConnection(ConnectionInput connectionInput)
		{
			var loadCases = new List<LoadCase>();

			var loadImpulses = new List<LoadImpulse>();

			foreach(var mem in connectionInput.Members)
			{
				loadImpulses.Add(new LoadImpulse() { MemberId = mem.Id, Position = 0, N = 1 });
				if(mem.IsContinuous)
				{
					loadImpulses.Add(new LoadImpulse() { MemberId = mem.Id, Position = 1, N = 1 });
				}
			}

			var loadCase = new LoadCase() { Id = 1, Name = "LC1", LoadImpulses = loadImpulses };
			loadCases.Add(loadCase);

			var res = new LoadingInput() {LoadCases = loadCases };
			return res;
		}

		private void CreateConnectionPoint(FeaModel feaModel, ConnectionInput connectionInput)
		{
			// Get the connection node (node at origin - ID = 1)
			var connectionNode = feaModel.Nodes[ConnectionPointId];

			// Create list of connected members
			var connectedMembers = new List<IIdeaConnectedMember>();

			// Add all members from FeaModel to the connection
			// The IsContinuous flag is taken from the corresponding member in ConnectionInput
			int memberIndex = 0;
			foreach (var member in feaModel.Members1D.Values)
			{
				// Get the corresponding member input to determine if it's continuous
				var memberInput = connectionInput.Members[memberIndex];
				
				var connectedMember = new IdeaConnectedMember(member.Id.ToString())
				{
					IdeaMember = member,
					GeometricalType = memberInput.IsContinuous ? IdeaGeometricalType.Continuous : IdeaGeometricalType.Ended,
					ConnectedMemberType = IdeaConnectedMemberType.Structural
				};

				connectedMembers.Add(connectedMember);

				memberIndex++;
			}

			// Create connection point with the list of connected members
			var connectionPoint = new IdeaConnectionPoint(ConnectionPointId)
			{
				Node = connectionNode,
				ConnectedMembers = connectedMembers
			};

			feaModel.ConnectionPoints.Add(ConnectionPointId, connectionPoint);
		}
	}
}
