using ConnectionIomGenerator.Fea;
using ConnectionIomGenerator.Model;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Numerics;

namespace ConnectionIomGenerator.Service
{
	/// <summary>
	/// Generates a Finite Element Analysis (FEA) model for steel connections from simplified connection input parameters.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="FeaGenerator"/> class converts high-level connection definitions (<see cref="ConnectionInput"/>) 
	/// into a detailed FEA model (<see cref="FeaModel"/>) that can be used with IDEA StatiCa BIM API. It automatically 
	/// generates all necessary components including nodes, elements, segments, members, and connection points.
	/// </para>
	/// <para><b>Key Responsibilities:</b></para>
	/// <list type="bullet">
	/// <item><description>Creates FEA nodes positioned in 3D space based on member directions and rotations</description></item>
	/// <item><description>Generates line segments representing member geometry</description></item>
	/// <item><description>Creates 1D elements with assigned cross-sections and materials</description></item>
	/// <item><description>Assembles members from elements (single element for ended, two elements for continuous)</description></item>
	/// <item><description>Establishes connection points with proper member connections</description></item>
	/// <item><description>Generates default loading patterns with unit loads</description></item>
	/// </list>
	/// <para><b>Member Types:</b></para>
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Ended Members</term>
	/// <description>Single element from connection point to end node (3m length)</description>
	/// </item>
	/// <item>
	/// <term>Continuous Members</term>
	/// <description>Two elements: connection point to +3m and connection point to -3m along member axis</description>
	/// </item>
	/// </list>
	/// <para><b>Coordinate System:</b></para>
	/// <para>
	/// The generator uses a global coordinate system with origin at (0,0,0) where all members connect.
	/// Each member's local coordinate system is calculated using three rotation angles:
	/// </para>
	/// <list type="number">
	/// <item><description><b>Direction</b> - Rotation around global Z-axis (0° = +X direction)</description></item>
	/// <item><description><b>Pitch</b> - Rotation around Y-axis (vertical tilt)</description></item>
	/// <item><description><b>Rotation</b> - Rotation around member's local X-axis (cross-section orientation)</description></item>
	/// </list>
	/// <para><b>Default Settings:</b></para>
	/// <list type="bullet">
	/// <item><description>Connection point ID: 1</description></item>
	/// <item><description>Origin: (0, 0, 0)</description></item>
	/// <item><description>Member length: 3.0 meters</description></item>
	/// <item><description>Material type: Steel</description></item>
	/// </list>
	/// </remarks>
	/// <example>
	/// <para>Basic usage to generate FEA model:</para>
	/// <code language="csharp">
	/// var connectionInput = new ConnectionInput
	/// {
	///     Material = "S355",
	///     Members = new List&lt;MemberInput&gt;
	/// {
	///         new MemberInput 
	///         { 
	///    Id = 1, 
	///    Name = "Column", 
	///     CrossSection = "HEB300",
	///             Direction = 0,      // Along +X axis
	///             Pitch = 90,       // Vertical (90° pitch)
	///      Rotation = 0,
	///             IsContinuous = true // Continuous column
	///         },
	///         new MemberInput 
	///         { 
	///       Id = 2, 
	///        Name = "Beam", 
	///         CrossSection = "IPE200",
	///          Direction = 90,     // Along +Y axis
	///      Pitch = 0,   // Horizontal
	///           Rotation = 0,
	///      IsContinuous = false // Ended beam
	///         }
	///     }
	/// };
	/// 
	/// var generator = new FeaGenerator();
	/// var feaModel = generator.Generate(connectionInput, null);
	/// 
	/// // feaModel now contains:
	/// // - 1 material (S355)
	/// // - 2 cross-sections (HEB300, IPE200)
	/// // - 4 nodes (1 connection point + 3 member ends)
	/// // - 3 elements (2 for continuous column, 1 for ended beam)
	/// // - 2 members
	/// // - 1 connection point
	/// </code>
	/// <para>Generate default loading:</para>
	/// <code language="csharp">
	/// var loadingInput = generator.CreateLoadingForConnection(connectionInput);
	/// var feaModel = generator.Generate(connectionInput, loadingInput);
	/// 
	/// // feaModel includes load case with unit loads on all member ends
	/// </code>
	/// </example>
	public class FeaGenerator
	{
		/// <summary>
		/// The unique identifier for the connection point where all members meet.
		/// </summary>
		static readonly int ConnectionPointId = 1;
		
		/// <summary>
		/// The origin point (0, 0, 0) in global coordinates where the connection is located.
		/// </summary>
		static readonly Vector3 Origin = new Vector3(0, 0, 0);
		
		/// <summary>
		/// Unit vector along the global X-axis.
		/// </summary>
		static readonly Vector3 AxisX = new Vector3(1,0,0);
		
		/// <summary>
		/// Unit vector along the global Y-axis.
		/// </summary>
		static readonly Vector3 AxisY = new Vector3(0, 1, 0);
		
		/// <summary>
		/// Unit vector along the global Z-axis.
		/// </summary>
		static readonly Vector3 AxisZ = new Vector3(0, 0, 1);
		
		/// <summary>
		/// Default length of generated members in meters. Members extend 3m from the connection point.
		/// </summary>
		static readonly float MemberLength = 3f;

		/// <summary>
		/// Initializes a new instance of the <see cref="FeaGenerator"/> class.
		/// </summary>
		public FeaGenerator() { }

		/// <summary>
		/// Generates a complete FEA model from connection input parameters.
		/// </summary>
		/// <param name="connectionInput">The connection definition containing material, members, and their geometric properties.</param>
		/// <param name="loadingInput">Optional loading definition. If provided, load cases are added to the FEA model. Can be null.</param>
		/// <returns>
		/// A fully configured <see cref="FeaModel"/> containing all nodes, elements, segments, members, 
		/// connection points, and optionally load cases.
		/// </returns>
		/// <remarks>
		/// <para><b>Generation Process:</b></para>
		/// <list type="number">
		/// <item><description><b>Material Creation</b> - Creates steel material from <paramref name="connectionInput"/>.Material</description></item>
		/// <item><description><b>Cross-Section Creation</b> - Extracts unique cross-section names and creates IdeaCrossSectionByName instances</description></item>
		/// <item><description><b>Connection Node</b> - Creates central node at origin (0,0,0)</description></item>
		/// <item><description><b>Member Generation</b> - For each member in <paramref name="connectionInput"/>.Members:
		///     <list type="bullet">
		///   <item><description>Calculates local coordinate system from Direction, Pitch, and Rotation angles</description></item>
		///     <item><description>Creates end nodes at ±3m along member axis</description></item>
		///     <item><description>Generates line segments connecting nodes</description></item>
		///     <item><description>Creates 1D elements with assigned cross-sections</description></item>
		///     <item><description>Assembles member (1 element if ended, 2 elements if continuous)</description></item>
		///     </list>
		/// </description></item>
		/// <item><description><b>Connection Point Creation</b> - Creates connection point at origin with all connected members</description></item>
		/// <item><description><b>Loading</b> - If <paramref name="loadingInput"/> is not null, adds load cases to the model</description></item>
		/// </list>
		/// <para><b>Coordinate System Calculation:</b></para>
		/// <para>
		/// Member local axes are calculated through sequential rotations:
		/// </para>
		/// <code>
		/// 1. Rotation around Y-axis by -Pitch angle
		/// 2. Rotation around Z-axis by Direction angle  
		/// 3. Rotation around local X-axis by Rotation angle
		/// </code>
		/// <para><b>Node Positioning:</b></para>
		/// <list type="bullet">
		/// <item><description>Ended members: Connection point (0,0,0) → End point (+3m along axis)</description></item>
		/// <item><description>Continuous members: Begin point (+3m) → Connection point (0,0,0) → End point (-3m)</description></item>
		/// </list>
		/// </remarks>
		/// <example>
		/// <para>Generate model with loading:</para>
		/// <code language="csharp">
		/// var input = new ConnectionInput 
		/// { 
		///     Material = "S235",
		///     Members = new List&lt;MemberInput&gt; { ... }
		/// };
		/// 
		/// var loading = new LoadingInput
		/// {
		///     LoadCases = new List&lt;LoadCase&gt;
		///{
		///         new LoadCase { Id = 1, Name = "Dead Load" }
		///     }
		/// };
		/// 
		/// var generator = new FeaGenerator();
		/// var feaModel = generator.Generate(input, loading);
		/// 
		/// Console.WriteLine($"Nodes: {feaModel.Nodes.Count}");
		/// Console.WriteLine($"Members: {feaModel.Members1D.Count}");
		/// Console.WriteLine($"Load Cases: {feaModel.Loading.Count}");
		/// </code>
		/// </example>
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
				float pitchInRadians = -1 * memInput.Pitch * MathF.PI / 180f;
				Matrix4x4 rotationAroundY = Matrix4x4.CreateFromAxisAngle(AxisY, pitchInRadians);

				var rotatedVectX =  Vector3.Transform(AxisX, rotationAroundY);
				var rotatedVectY = Vector3.Transform(AxisY, rotationAroundY);
				var rotatedVectZ = Vector3.Transform(AxisZ, rotationAroundY);

				// Rotation around Z axis (from Rotation property)
				float rotationInRadians = memInput.Direction * MathF.PI / 180f;
				Matrix4x4 rotationAroundZ = Matrix4x4.CreateFromAxisAngle(AxisZ, rotationInRadians);

				var vecX = Vector3.Transform(rotatedVectX, rotationAroundZ);
				var vecY = Vector3.Transform(rotatedVectY, rotationAroundZ);
				var vecZ = Vector3.Transform(rotatedVectZ, rotationAroundZ);

				// rotation around local axis X
				float rotationXInRadians = memInput.Rotation * MathF.PI / 180f;
				Matrix4x4 rotationAroundLocalX = Matrix4x4.CreateFromAxisAngle(vecX, rotationXInRadians);

				vecY = Vector3.Transform(vecY, rotationAroundLocalX);
				vecZ = Vector3.Transform(vecZ, rotationAroundLocalX);

				CoordSystemByVector lcs = new CoordSystemByVector()
				{
					VecX = new Vector3D(vecX.X, vecX.Y, vecX.Z),
					VecY = new Vector3D(vecY.X, vecY.Y, vecY.Z),
					VecZ = new Vector3D(vecZ.X, vecZ.Y, vecZ.Z)
				};

				dir = vecX;

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

		/// <summary>
		/// Creates a default loading pattern for the connection with unit loads on all member ends.
		/// </summary>
		/// <param name="connectionInput">The connection definition for which to create loading.</param>
		/// <returns>
		/// A <see cref="LoadingInput"/> containing a single load case "LC1" with unit axial loads (N=1) 
		/// applied at all member end positions.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method generates a simple loading scenario suitable for preliminary connection analysis.
		/// The loading includes:
		/// </para>
		/// <list type="bullet">
		/// <item><description>One load case named "LC1" with ID = 1</description></item>
		/// <item><description>For each member: unit axial force (N = 1 kN) at position 0 (connection point)</description></item>
		/// <item><description>For continuous members: additional unit load at position 1 (opposite end)</description></item>
		/// </list>
		/// <para><b>Load Positions:</b></para>
		/// <list type="table">
		/// <listheader>
		/// <term>Position</term>
		/// <description>Location</description>
		/// </listheader>
		/// <item>
		/// <term>0</term>
		/// <description>At the connection point (start of member)</description>
		/// </item>
		/// <item>
		/// <term>1</term>
		/// <description>At the opposite end (only for continuous members)</description>
		/// </item>
		/// </list>
		/// <para>
		/// The generated loading is suitable for:
		/// </para>
		/// <list type="bullet">
		/// <item><description>Testing connection behavior under simple loads</description></item>
		/// <item><description>Verifying load transfer paths</description></item>
		/// <item><description>Initial connection design validation</description></item>
		/// </list>
		/// </remarks>
		/// <example>
		/// <para>Generate default loading for a connection:</para>
		/// <code language="csharp">
		/// var connectionInput = new ConnectionInput
		/// {
		///     Material = "S355",
		///     Members = new List&lt;MemberInput&gt;
		///     {
		///      new MemberInput { Id = 1, IsContinuous = true },  // 2 load impulses
		///         new MemberInput { Id = 2, IsContinuous = false }  // 1 load impulse
		///     }
		/// };
		/// 
		/// var generator = new FeaGenerator();
		/// var loading = generator.CreateLoadingForConnection(connectionInput);
		/// 
		/// // loading contains:
		/// // - 1 load case "LC1"
		/// // - 3 load impulses total (2 for continuous member + 1 for ended member)
		/// // - All with N = 1 kN
		/// 
		/// var feaModel = generator.Generate(connectionInput, loading);
		/// </code>
		/// </example>
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

		/// <summary>
		/// Creates a connection point at the origin with all generated members connected.
		/// </summary>
		/// <param name="feaModel">The FEA model to which the connection point will be added.</param>
		/// <param name="connectionInput">The connection input providing member continuity information.</param>
		/// <remarks>
		/// <para>
		/// This private helper method establishes the connection point where all structural members meet.
		/// It performs the following operations:
		/// </para>
		/// <list type="number">
		/// <item><description>Retrieves the connection node at origin (ID = 1)</description></item>
		/// <item><description>Creates <see cref="IdeaConnectedMember"/> instances for each member in the FEA model</description></item>
		/// <item><description>Sets geometric type based on member continuity (Continuous or Ended)</description></item>
		/// <item><description>Sets connected member type as Structural for all members</description></item>
		/// <item><description>Creates and adds the connection point to the FEA model</description></item>
		/// </list>
		/// <para><b>Member Classification:</b></para>
		/// <list type="bullet">
		/// <item><description><b>Continuous Members</b> - <see cref="IdeaGeometricalType.Continuous"/> - Member continues through connection</description></item>
		/// <item><description><b>Ended Members</b> - <see cref="IdeaGeometricalType.Ended"/> - Member terminates at connection</description></item>
		/// </list>
		/// <para>
		/// All members are marked as <see cref="IdeaConnectedMemberType.Structural"/>, indicating they 
		/// participate in load transfer through the connection.
		/// </para>
		/// <para><b>Important:</b></para>
		/// <para>
		/// The order of members in <paramref name="feaModel"/>.Members1D must match the order in 
		/// <paramref name="connectionInput"/>.Members to correctly assign continuity properties.
		/// </para>
		/// </remarks>
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
