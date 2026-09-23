using CI.Geometry3D;
using IdeaStatiCa.BIM.Common;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Exeptions;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using WM = System.Windows.Media.Media3D;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utilities
{
	internal static class BulkSelectionHelper
	{
		internal const string HaunchMemberName = "HAUNCH";
		internal const string TeklaAnchorRodName = "ANCHOR ROD";
		internal const string TeklaAnchorWasherName = "WASHER";
		internal const string TeklaAnchorNutName = "NUT";
		/// <summary>
		/// Find Joints
		/// </summary>
		/// <param name="myModel"></param>
		/// <param name="partsEnumerator"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static SorterResult FindJoints(Tekla.Structures.Model.Model myModel, List<ModelObject> partsEnumerator, BIM.Common.SorterSettings settings = null, IPluginLogger plugInLogger = null)
		{
			List<BIM.Common.Member> bMembers = new List<BIM.Common.Member>();
			// Tekla identity -> the item built for it, so a bolt group can be given the parts it clamps once they all exist.
			var itemsByPart = new Dictionary<string, List<BIM.Common.Item>>();
			void Register(ModelObject source, BIM.Common.Item item)
			{
				var key = source.Identifier.GUID.ToString();
				if (!itemsByPart.TryGetValue(key, out var built))
				{
					built = new List<BIM.Common.Item>();
					itemsByPart[key] = built;
				}
				built.Add(item);
			}
			var clampedPartsByFastener = new Dictionary<BIM.Common.FastenerGrid, List<string>>();
			List<BIM.Common.Plate> plates = new List<BIM.Common.Plate>();
			List<BIM.Common.Weld> welds = new List<BIM.Common.Weld>();
			List<BIM.Common.FastenerGrid> fasteners = new List<BIM.Common.FastenerGrid>();

			Item.CustomComparer = new ItemEqualityComparer();

			foreach (var currentPart in partsEnumerator)
			{
				if (currentPart is Beam beam)
				{
					var partLcs = BulkSelectionHelper.CreateMatrix(beam);
					var bb = BulkSelectionHelper.CreateOrientedBoundingBox(myModel, beam);
					System.Windows.Rect cssBounds = new System.Windows.Rect(new System.Windows.Point(-1 * bb.Extent2, -1 * bb.Extent1), new System.Windows.Point(bb.Extent2, bb.Extent1));

					//for concrete block take smaller bounding box
					if (IdentifierHelper.ConcreteBlocksFilter(beam))
					{
						cssBounds = new System.Windows.Rect(new System.Windows.Point(-0.3 * bb.Extent2, -0.3 * bb.Extent1), new System.Windows.Point(0.3 * bb.Extent2, 0.3 * bb.Extent1));
					}

					var cl1 = beam.GetCenterLine(false).OfType<Point>().ToArray();

					var begin = new Point3D(cl1[0].X, cl1[0].Y, cl1[0].Z);
					var end = new Point3D(cl1[1].X, cl1[1].Y, cl1[1].Z);

					if (IsRectangularCssBeam(beam) && IsMadeByAConnectionComponent(beam))
					{
						var plateItem = BuildPlateFromRectangularBeam(myModel, beam, partLcs, begin, end);
						Register(beam, plateItem);
						plates.Add(plateItem);
						// A part read as a plate no longer ends a member, so it seeds no node - a joint that existed only
						// because this part ended there stops forming. That is the intent, and it is also the first thing to
						// look at when a connection is reported missing, so say which parts moved and how thick they came out.
						var madeBy = beam.GetFatherComponent();
						plugInLogger?.LogDebug($"FindJoints read as a plate rather than a member: '{beam.Name}' profile '{beam.Profile?.ProfileString}' thickness {plateItem.Thickness:F1} madeBy {madeBy?.GetType().Name ?? "(none)"} '{madeBy?.Name}' guid {beam.Identifier.GUID}");
						continue;
					}
					var beamItem = new BIM.Common.Member(beam, partLcs, begin, end, cssBounds);
					Register(beam, beamItem);
					bMembers.Add(beamItem);
				}

				if (currentPart is PolyBeam polyBeam)
				{
					var partLcs = BulkSelectionHelper.CreateMatrix(polyBeam);
					System.Windows.Rect cssBounds = new System.Windows.Rect(new System.Windows.Point(-1 * 200, -1 * 200), new System.Windows.Point(200, 200));

					var cl1 = polyBeam.GetCenterLine(false).OfType<Point>().ToArray();

					var begin = new Point3D(cl1[0].X, cl1[0].Y, cl1[0].Z);
					var end = new Point3D(cl1[1].X, cl1[1].Y, cl1[1].Z);

					var polyBeamItem = new BIM.Common.Member(polyBeam, partLcs, begin, end, cssBounds);
					Register(polyBeam, polyBeamItem);
					bMembers.Add(polyBeamItem);
				}

				if (currentPart is ContourPlate contourPlate)
				{
					Matrix44 lcs = BulkSelectionHelper.CreateMatrix(contourPlate);

					var points = BulkSelectionHelper.GetContourPlatePoints(contourPlate);

					var plateItem = new BIM.Common.Plate(contourPlate, lcs, points, BulkSelectionHelper.GetContourPlateThickness(contourPlate));
					Register(contourPlate, plateItem);
					plates.Add(plateItem);
				}

				if (currentPart is BoltGroup boltGroup)
				{
					Matrix44 lcs = BulkSelectionHelper.CreateMatrix(boltGroup);
					var boltPositions = BulkSelectionHelper.GetBoltPositions(boltGroup);

					var fastener = new BIM.Common.FastenerGrid(boltGroup, lcs, boltPositions);
					clampedPartsByFastener[fastener] = ClampedPartIds(boltGroup);
					fasteners.Add(fastener);
				}

				if (currentPart is BentPlate bentPlate)
				{
					GeometrySectionEnumerator geometryEnumerator = bentPlate.Geometry.GetGeometryEnumerator();
					while (geometryEnumerator.MoveNext())
					{
						if (geometryEnumerator.Current?.GeometryNode is PolygonNode node)
						{
							var tuple = BulkSelectionHelper.GetPlateDataFromPolygon(node, bentPlate);
							var bentItem = new BIM.Common.Plate(bentPlate, tuple.Item1, tuple.Item2, bentPlate.Thickness);
							Register(bentPlate, bentItem);
							plates.Add(bentItem);
						}
					}
				}

				if (currentPart is BaseWeld baseWeld)
				{
					welds.Add(new BIM.Common.Weld(baseWeld, BulkSelectionHelper.GetBIMItem(myModel, baseWeld.MainObject), BulkSelectionHelper.GetBIMItem(myModel, baseWeld.SecondaryObject)));
				}

				if (currentPart is Part part)
				{
					var weldsSet = part.GetWelds();
					while (weldsSet.MoveNext())
					{
						var modelObj = weldsSet.Current;

						if (modelObj is BaseWeld weld)
						{
							welds.Add(new BIM.Common.Weld(weld, BulkSelectionHelper.GetBIMItem(myModel, weld.MainObject), BulkSelectionHelper.GetBIMItem(myModel, weld.SecondaryObject)));
						}
					}

					var bolts = part.GetBolts();

					while (bolts.MoveNext())
					{
						var modelObj = bolts.Current;
						if (modelObj is BoltGroup boltGroupPart)
						{
							Matrix44 lcs = BulkSelectionHelper.CreateMatrix(boltGroupPart);
							var boltPositions = BulkSelectionHelper.GetBoltPositions(boltGroupPart);
							var fastener = new BIM.Common.FastenerGrid(boltGroupPart, lcs, boltPositions);
							clampedPartsByFastener[fastener] = ClampedPartIds(boltGroupPart);
							fasteners.Add(fastener);
						}
					}
				}
			}

			ResolveClampedItems(clampedPartsByFastener, itemsByPart, plugInLogger);

			var sorterData = new BIM.Common.SorterData
			{
				Members = bMembers,
				Plates = plates,
				Welds = welds,
				Fasteners = fasteners
			};

			var sorter = new BIM.Common.ItemsSorter();
			if (settings == null)
			{
				settings = new BIM.Common.SorterSettings
				{
					EnlargeNodeXin = 1.6,
					EnlargeNodeXout = 1.6,
					EnlargeNodeY = 1.7,
					EnlargeNodeZ = 1.7,
				};
			}

			var sortedJoints = sorter.Sort(sorterData, settings);

			ReportItemsNoJointTook(sorterData, sortedJoints, plugInLogger);

			//Test of uncontrolled greedy alg
			// by discussion threshold is 20 members in connection
			if (sortedJoints.Joints.Count == 1 && sortedJoints.Joints[0].Members.Count > 20)
			{
				throw new BulkSelectionOverflowException(sortedJoints.Joints[0].Members.Count);
			}

			return sortedJoints;
		}

		/// <summary>
		/// The Tekla identities of the parts a bolt group clamps. Read here rather than in the importer because the
		/// sorter needs them to place a plate the node box did not reach, which happens before any import runs.
		/// </summary>
		private static List<string> ClampedPartIds(BoltGroup boltGroup)
			=> PartsBoltedBy(boltGroup)
				.Where(bolted => bolted.Part != null)
				.Select(bolted => bolted.Part.Identifier.GUID.ToString())
				.ToList();

		/// <summary>
		/// The slots a bolt group names, in the order Tekla exposes them. Two readers need this - one to learn which
		/// items the group clamps together, one to fill the group's connected parts on the way out - and a property
		/// Tekla adds later has to reach both or the two answers drift apart.
		/// <para>
		/// A named slot holding no part is yielded with a null part rather than dropped: a group that names nothing is
		/// a group short of an operand, and that is worth reporting rather than passing over in silence.
		/// </para>
		/// </summary>
		internal static IEnumerable<(Part Part, string Role)> PartsBoltedBy(BoltGroup boltGroup)
		{
			yield return (boltGroup.PartToBoltTo as Part, nameof(boltGroup.PartToBoltTo));
			yield return (boltGroup.PartToBeBolted as Part, nameof(boltGroup.PartToBeBolted));

			if (boltGroup.OtherPartsToBolt == null)
			{
				yield break;
			}

			foreach (var other in boltGroup.OtherPartsToBolt)
			{
				if (other is Part otherPart)
				{
					yield return (otherPart, nameof(boltGroup.OtherPartsToBolt));
				}
			}
		}

		/// <summary>
		/// Hands each fastener the items for the parts it names. A part the source names but the selection does not
		/// contain has no item to hand over, and the fastener is then one reference short of placing it - so it is
		/// named rather than passed over in silence. One source part can build several items (a bent plate becomes
		/// one plate per face), and every one of them is clamped.
		/// </summary>
		private static void ResolveClampedItems(
			IReadOnlyDictionary<BIM.Common.FastenerGrid, List<string>> clampedPartsByFastener,
			IReadOnlyDictionary<string, List<BIM.Common.Item>> itemsByPart,
			IPluginLogger plugInLogger)
		{
			foreach (var pair in clampedPartsByFastener)
			{
				foreach (var partId in pair.Value)
				{
					if (itemsByPart.TryGetValue(partId, out var built))
					{
						pair.Key.ClampedItems.AddRange(built);
						continue;
					}
					plugInLogger?.LogInformation($"Bolt group {(pair.Key.Parent as ModelObject)?.Identifier.GUID} names part {partId}, which is not among the selected parts - it cannot be recovered through this group");
				}
			}
		}

		/// <summary>
		/// Names every selected part no joint claimed. A part the user selected but no node box reached is not taken,
		/// so it never widens the box toward itself and stays untaken - and it is then absent from the model entirely,
		/// which downstream reads as a bolt grid or weld holding one part rather than as a plate that went missing.
		/// Sort replaces the collections on <paramref name="sorterData"/> with their de-duplicated form, so what is
		/// compared here is what was actually sorted.
		/// </summary>
		private static void ReportItemsNoJointTook(BIM.Common.SorterData sorterData, BIM.Common.SorterResult sortedJoints, IPluginLogger plugInLogger)
		{
			if (plugInLogger == null)
			{
				return;
			}

			var taken = new HashSet<BIM.Common.Item>(sortedJoints.Joints
				.SelectMany(j => j.Members.Cast<BIM.Common.Item>()
					.Concat(j.StiffeningMembers)
					.Concat(j.Plates)
					.Concat(j.Welds)
					.Concat(j.Fasteners)));

			var selected = (sorterData.Members ?? Enumerable.Empty<BIM.Common.Member>()).Cast<BIM.Common.Item>()
				.Concat(sorterData.Plates ?? Enumerable.Empty<BIM.Common.Plate>())
				.Concat(sorterData.Fasteners ?? Enumerable.Empty<BIM.Common.FastenerGrid>())
				.Concat(sorterData.Welds ?? Enumerable.Empty<BIM.Common.Weld>());

			foreach (var item in selected)
			{
				if (taken.Contains(item))
				{
					continue;
				}

				plugInLogger.LogInformation($"FindJoints selected but no joint took it: {item.GetType().Name} {Describe(item.Parent as ModelObject)}");
			}
		}

		/// <summary>
		/// Enough of a Tekla object to find it again. A fastener or a weld is not a <see cref="Part"/> and has neither
		/// a name nor a profile, so only the guid identifies it - and those are the items most worth naming here.
		/// </summary>
		private static string Describe(ModelObject source)
		{
			if (source is Part part)
			{
				return $"'{part.Name}' profile '{part.Profile?.ProfileString}' guid {part.Identifier.GUID}";
			}

			return $"'{source?.GetType().Name}' guid {source?.Identifier.GUID}";
		}

		/// <summary>
		/// Whether a Tekla CONNECTION component created this part. Such a part is connection detailing - a splice
		/// plate, a gusset, a haunch web - while the same profile drawn by hand is as likely to be a frame member,
		/// and only the source can tell the two apart.
		/// <para>
		/// A DETAIL component is excluded, and that is the whole reason this asks for the kind rather than merely for
		/// a component: a base plate comes from one, and nothing else frames into the foot of a column, so the base
		/// plate counting as a member is what gives that node a second one and makes it a joint at all. Read as a
		/// plate it would seed no node, and the column base would stop being a connection.
		/// <para>
		/// A haunch is admitted by the name of the component that made it, whatever kind that component is: its web is
		/// a plate by construction, and it is not always built by a connection.
		/// </para>
		/// </para>
		/// </summary>
		private static bool IsMadeByAConnectionComponent(Beam beam)
		{
			var father = beam.GetFatherComponent();

			return father is Connection || father?.Name.ToUpper() == HaunchMemberName;
		}

		/// <summary>
		/// The plate a beam with a rectangular profile really is. The thinner of the two cross-section directions is
		/// the plate's normal - whichever local axis that turns out to be - and the contour spans the other one.
		/// <para>
		/// The dimensions come from an uninflated box on purpose: the box the node search uses doubles any half-extent
		/// under 50 mm, which is every plate's thickness.
		/// </para>
		/// </summary>
		private static BIM.Common.Plate BuildPlateFromRectangularBeam(
			Tekla.Structures.Model.Model model, Beam beam, Matrix44 partLcs, IPoint3D begin, IPoint3D end)
		{
			var bb = CreateOrientedBoundingBox(model, beam, inflateSmallExtents: false);
			var across = CrossSectionHalfExtents(extentAcrossTeklaY: bb.Extent1, extentAcrossTeklaZ: bb.Extent2);
			var plate = PlateFromCrossSection(partLcs, begin, end, across.AcrossY, across.AcrossZ);

			return new BIM.Common.Plate(beam, partLcs, plate.Contour, plate.Thickness);
		}

		/// <summary>
		/// A part's two cross-section half-extents, named for the axes of the matrix <see cref="CreateMatrix"/> builds
		/// rather than for Tekla's own. The two cross: the box measures <c>Extent1</c> across Tekla's Y and
		/// <c>Extent2</c> across Tekla's Z, while the matrix takes Tekla's Y as its Z axis and Tekla's Z as its Y.
		/// <para>
		/// Its own function because handing the two over in Tekla's order instead leaves every dimension reading
		/// correctly - the width, the length and the thickness all come out right - while the plate lies in the plane
		/// of its own normal.
		/// </para>
		/// </summary>
		internal static (double AcrossY, double AcrossZ) CrossSectionHalfExtents(double extentAcrossTeklaY, double extentAcrossTeklaZ)
			=> (extentAcrossTeklaZ, extentAcrossTeklaY);

		/// <summary>
		/// The contour and thickness of the plate a part with the given cross-section half-extents is. The thinner
		/// direction is the plate's normal, whichever local axis it falls on; the contour is the rectangle the other
		/// one sweeps from <paramref name="begin"/> to <paramref name="end"/>.
		/// </summary>
		internal static (List<IPoint3D> Contour, double Thickness) PlateFromCrossSection(
			Matrix44 partLcs, IPoint3D begin, IPoint3D end, double halfExtentAcrossY, double halfExtentAcrossZ)
		{
			var inPlane = halfExtentAcrossY >= halfExtentAcrossZ ? partLcs.AxisY : partLcs.AxisZ;
			var halfWidth = Math.Max(halfExtentAcrossY, halfExtentAcrossZ);
			var thickness = 2 * Math.Min(halfExtentAcrossY, halfExtentAcrossZ);

			var toOneEdge = (inPlane * halfWidth).ToMediaVector();
			var toOther = (inPlane * -halfWidth).ToMediaVector();

			var b1 = begin.ToMediaPoint() + toOneEdge;
			var b2 = begin.ToMediaPoint() + toOther;
			var b3 = end.ToMediaPoint() + toOneEdge;
			var b4 = end.ToMediaPoint() + toOther;

			var contour = new List<IPoint3D>() { b1.ToIndoPoint3D(), b3.ToIndoPoint3D(), b4.ToIndoPoint3D(), b2.ToIndoPoint3D() };

			return (contour, thickness);
		}

		/// <summary>
		/// Is rectangular css of beam
		/// </summary>
		/// <param name="beam"></param>
		/// <returns></returns>
		public static bool IsRectangularCssBeam(Part beam)
		{
			string strProfName = beam.Profile.ProfileString;

			LibraryProfileItem profileItem = new LibraryProfileItem();
			profileItem.Select(strProfName);

			if (profileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN)
			{
				ParametricProfileItem paramProfileItem = new ParametricProfileItem();
				if (paramProfileItem.Select(strProfName) && paramProfileItem.ProfileItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_PL)
				{
					return true;
				}
			}
			return false;
		}

		private static BIM.Common.Item GetBIMItem(Tekla.Structures.Model.Model myModel, ModelObject item)
		{
			if (item is ContourPlate contourPlate)
			{
				Matrix44 lcsItem1 = CreateMatrix(contourPlate);
				var points = GetContourPlatePoints(contourPlate);
				return new BIM.Common.Plate(contourPlate, lcsItem1, points, GetContourPlateThickness(contourPlate));
			}
			else if (item is Beam beam)
			{
				Matrix44 lcsItem1 = CreateMatrix(beam);

				var bb = CreateOrientedBoundingBox(myModel, beam);

				var begin = new CI.Geometry3D.Point3D(beam.StartPoint.X, beam.StartPoint.Y, beam.StartPoint.Z);
				var end = new CI.Geometry3D.Point3D(beam.EndPoint.X, beam.EndPoint.Y, beam.EndPoint.Z);
				System.Windows.Rect cssBounds = new System.Windows.Rect(new System.Windows.Point(-0.5 * bb.Extent2, -0.5 * bb.Extent1), new System.Windows.Point(0.5 * bb.Extent2, 0.5 * bb.Extent1));

				return new BIM.Common.Member(beam, lcsItem1, begin, end, cssBounds);
			}
			else
			{
				throw new ArgumentException("GetBIMItem - Not found item" + item.ToString());
			}
		}

		private static List<CI.Geometry3D.Point3D> GetBoltPositions(BoltGroup boltGroup)
		{
			var midPoints = boltGroup.BoltPositions;

			List<CI.Geometry3D.Point3D> points = new List<CI.Geometry3D.Point3D>();
			foreach (var p in midPoints)
			{
				Point point = p as Point;
				points.Add(new CI.Geometry3D.Point3D() { X = point.X, Y = point.Y, Z = point.Z });
			}
			return points;
		}

		private static Tuple<Matrix44, List<CI.Geometry3D.IPoint3D>> GetPlateDataFromPolygon(PolygonNode node, BentPlate bentPlate)
		{
			var partCs = bentPlate.GetCoordinateSystem();
			Vector axisZ = Vector.Cross(partCs.AxisX, partCs.AxisY);
			WM.Vector3D pltAxisZ = new WM.Vector3D(axisZ.X, axisZ.Y, axisZ.Z);
			pltAxisZ.Normalize();

			List<CI.Geometry3D.IPoint3D> contourPoints = new List<CI.Geometry3D.IPoint3D>();
			foreach (ContourPoint point in node.Contour.ContourPoints)
			{
				contourPoints.Add(new CI.Geometry3D.Point3D(point.X, point.Y, point.Z));
			}

			List<CI.Geometry3D.IPoint3D> cIPoints = PlateContour.FirstClosedContour(contourPoints).ToList();
			List<WM.Point3D> points = cIPoints.Select(p => new WM.Point3D(p.X, p.Y, p.Z)).ToList();

			WM.Vector3D translation = new WM.Vector3D(points[0].X, points[0].Y, points[0].Z);

			var proLcsYT = points[1] - points[0];
			proLcsYT.Normalize();
			var vYT = proLcsYT.ToIndoVector3D();

			proLcsYT = points[1] - points[0];
			var proLcsYT2 = points[2] - points[1];

			WM.Vector3D crossProduct = WM.Vector3D.CrossProduct(proLcsYT, proLcsYT2);
			if (crossProduct.Length == 0)
			{
				crossProduct = proLcsYT2;
			}
			var proLcsXT = crossProduct;
			proLcsXT.Normalize();
			var vZT = proLcsXT.ToIndoVector3D();
			var vXT = vZT * vYT;

			var pltMatrix = new Matrix44((WM.Point3D)translation, vXT, vYT, vZT);

			return new Tuple<Matrix44, List<IPoint3D>>(pltMatrix, cIPoints);
		}

		private static double GetContourPlateThickness(ContourPlate contourPlate)
		{
			const string WidthKey = "WIDTH";
			Hashtable propTable = new Hashtable();

			ArrayList PltThickness_ParamNames = new ArrayList
			{
				WidthKey
			};
			contourPlate.GetDoubleReportProperties(PltThickness_ParamNames, ref propTable);

			//Name = part.Name,
			return (double)propTable[WidthKey];
		}

		private static List<CI.Geometry3D.IPoint3D> GetContourPlatePoints(ContourPlate contourPlate)
		{
			var solid = contourPlate.GetSolid(Solid.SolidCreationTypeEnum.NORMAL);
			CoordinateSystem partCs = contourPlate.GetCoordinateSystem();
			Vector axisZ = Vector.Cross(partCs.AxisX, partCs.AxisY);
			WM.Vector3D pltAxisZ = new WM.Vector3D(axisZ.X, axisZ.Y, axisZ.Z);
			pltAxisZ.Normalize();

			var faceEnum = solid.GetFaceEnumerator();

			global::Tekla.Structures.Solid.Face foundFace = null;
			while (faceEnum.MoveNext())
			{
				WM.Vector3D tempFaceNormal = new WM.Vector3D(faceEnum.Current.Normal.X, faceEnum.Current.Normal.Y, faceEnum.Current.Normal.Z);
				tempFaceNormal.Normalize();

				if (tempFaceNormal.IsEqual(pltAxisZ))
				{
					foundFace = faceEnum.Current;
					break;
				}
			}

			if (foundFace == null)
			{
				foundFace = faceEnum.Current;
			}

			var loopEnumerator = foundFace.GetLoopEnumerator();
			if (!loopEnumerator.MoveNext())
			{
				throw new ArgumentException("Invalid loop");
			}

			var firstLoop = loopEnumerator.Current;

			var vertexEnumerator = firstLoop.GetVertexEnumerator();

			List<CI.Geometry3D.IPoint3D> points = new List<CI.Geometry3D.IPoint3D>();
			while (vertexEnumerator.MoveNext())
			{
				points.Add(new CI.Geometry3D.Point3D(vertexEnumerator.Current.X, vertexEnumerator.Current.Y, vertexEnumerator.Current.Z));
			}

			return PlateContour.FirstClosedContour(points).ToList();
		}

		/// <summary>
		/// The part's box in its own axes. <paramref name="inflateSmallExtents"/> is what the node search wants and
		/// what a real dimension must not have: it doubles any cross-section half-extent under 50 mm so a small part
		/// is easier to catch in a node box, which on a 19 mm plate doubles the thickness itself.
		/// </summary>
		private static OBB CreateOrientedBoundingBox(Tekla.Structures.Model.Model model, Beam beam, bool inflateSmallExtents = true)
		{
			OBB obb = null;

			if (beam != null)
			{
				WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
				TransformationPlane originalTransformationPlane = workPlaneHandler.GetCurrentTransformationPlane();

				Solid solid = beam.GetSolid();
				Point minPointInCurrentPlane = solid.MinimumPoint;
				Point maxPointInCurrentPlane = solid.MaximumPoint;

				Point centerPoint = CalculateCenterPoint(minPointInCurrentPlane, maxPointInCurrentPlane);

				CoordinateSystem coordSys = beam.GetCoordinateSystem();
				TransformationPlane localTransformationPlane = new TransformationPlane(coordSys);
				workPlaneHandler.SetCurrentTransformationPlane(localTransformationPlane);

				solid = beam.GetSolid();
				Point minPoint = solid.MinimumPoint;
				Point maxPoint = solid.MaximumPoint;
				double extent0 = (maxPoint.X - minPoint.X) / 2;
				double extent1 = (maxPoint.Y - minPoint.Y) / 2;
				double extent2 = (maxPoint.Z - minPoint.Z) / 2;

				//for non anchor beams increase size of BB for small items 
				if (inflateSmallExtents && beam.Name != TeklaAnchorRodName && beam.Name != TeklaAnchorWasherName && beam.Name != TeklaAnchorNutName)
				{
					if (extent1 < 50)
					{
						extent1 *= 2;
					}
					if (extent2 < 50)
					{
						extent2 *= 2;
					}
				}
				workPlaneHandler.SetCurrentTransformationPlane(originalTransformationPlane);

				obb = new OBB(centerPoint, coordSys.AxisX, coordSys.AxisY,
												coordSys.AxisX.Cross(coordSys.AxisY), extent0, extent1, extent2);
			}

			return obb;
		}

		private static Matrix44 CreateMatrix(ModelObject part)
		{
			var partLcs = part.GetCoordinateSystem();

			var origin = new Point3D(partLcs.Origin.X, partLcs.Origin.Y, partLcs.Origin.Z);
			var axisX = new Vector3D(partLcs.AxisX.X, partLcs.AxisX.Y, partLcs.AxisX.Z).Normalize;
			var axisZ = new Vector3D(partLcs.AxisY.X, partLcs.AxisY.Y, partLcs.AxisY.Z).Normalize;
			var axisY = (axisX * axisZ).Normalize;
			return new Matrix44(origin, axisX, axisY, axisZ);
		}

		private static Point CalculateCenterPoint(Point min, Point max)
		{
			double x = min.X + ((max.X - min.X) / 2);
			double y = min.Y + ((max.Y - min.Y) / 2);
			double z = min.Z + ((max.Z - min.Z) / 2);

			return new Point(x, y, z);
		}
	}

	/// <summary>
	/// Item Equality Comparer for item sorter
	/// </summary>
	public class ItemEqualityComparer : IEqualityComparer<Item>
	{
		public bool Equals(Item x, Item y)
		{
			return (x.Parent as ModelObject)?.Identifier.Equals((y.Parent as ModelObject)?.Identifier) ?? false;
		}

		public int GetHashCode(Item obj)
		{
			return (obj.Parent as ModelObject)?.Identifier.GetHashCode() ?? 0;
		}
	}
}
