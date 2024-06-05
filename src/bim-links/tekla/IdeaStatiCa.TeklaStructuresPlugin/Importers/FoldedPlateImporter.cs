using CI.Geometry3D;
using IdeaRS.OpenModel.Geometry2D;
using IdeaStatica.TeklaStructuresPlugin.Utils;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using System.Collections;
using System.Collections.Generic;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using WM = System.Windows.Media.Media3D;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class FoldedPlateImporter : BaseImporter<IIdeaFoldedPlate>
	{
		public FoldedPlateImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaFoldedPlate Create(string id)
		{
			PlugInLogger.LogInformation($"FoldedPlateImporter create {id}");
			var teklaItem = Model.GetItemByHandler(id);
			if (teklaItem == null)
			{
				PlugInLogger.LogInformation($"FoldedPlateImporter not found foldedPlate {id}");
				return null;
			}

			if (teklaItem is TSM.BentPlate bentPlate)
			{
				TSM.GeometrySectionEnumerator geometryEnumerator = bentPlate.Geometry.GetGeometryEnumerator();

				FoldedPlate foldedPlate = new FoldedPlate(bentPlate.Identifier.ID.ToString());

				IIdeaPlate previousPlate = null;
				IdeaBend previosBend = null;
				int numberOfPlates = 1;
				while (geometryEnumerator.MoveNext())
				{
					if (geometryEnumerator.Current != null)
					{
						if (geometryEnumerator.Current.GeometryNode is TSM.PolygonNode node)
						{
							Plate plate = GetPlateDataFromPolygon(node, bentPlate, numberOfPlates);
							previousPlate = plate;
							if (previosBend != null && previosBend.Plate2 == null)
							{
								previosBend.Plate2 = plate;
							}
							(foldedPlate.Plates as List<IIdeaPlate>)?.Add(plate);
							numberOfPlates++;
						}
						else if (geometryEnumerator.Current.GeometryNode is TSM.CylindricalSurfaceNode cNode)
						{
							IdeaBend bend = new IdeaBend(cNode.ToString());
							if (previousPlate != null)
							{
								bend.Plate1 = previousPlate;
							}
							bend.Radius = cNode.Surface.Radius.MilimetersToMeters();

							bend.LineOnSideBoundary1 = new BimApi.Segment3D(cNode.ToString() + "ls1")
							{
								StartNodeNo = Model.GetPointId(cNode.Surface.SideBoundary1.Point1),
								EndNodeNo = Model.GetPointId(cNode.Surface.SideBoundary1.Point2)
							};

							bend.LineOnSideBoundary2 = new BimApi.Segment3D(cNode.ToString() + "ls2")
							{
								StartNodeNo = Model.GetPointId(cNode.Surface.SideBoundary2.Point1),
								EndNodeNo = Model.GetPointId(cNode.Surface.SideBoundary2.Point2)
							};
							bend.EndFaceNormal = new IdeaVector3D(cNode.Surface.EndFaceNormal1.X, cNode.Surface.EndFaceNormal1.Y, cNode.Surface.EndFaceNormal1.Z);
							(foldedPlate.Bends as List<IIdeaBend>)?.Add(bend);

							previosBend = bend;
						}
					}
				}

				Model.CacheCreatedObject(new StringIdentifier<IIdeaFoldedPlate>(id), foldedPlate);
				return foldedPlate;
			}
			else
			{
				PlugInLogger.LogInformation($"FoldedPlateImporter not found foldedPlate {id} as {teklaItem.GetType()}");
				return null;
			}
		}

		private const string WidthKey = "WIDTH";
		private const double CssUnitScale = 1e-3;

		private Plate GetPlateDataFromPolygon(TSM.PolygonNode node, TSM.BentPlate bentPlate, int orderNumber)
		{
			string pltMaterial = bentPlate.Material.MaterialString;

			Hashtable propTable = new Hashtable();
			bentPlate.GetDoubleReportProperties(new ArrayList
			{
				WidthKey
			}, ref propTable);
			Plate plate = new Plate(bentPlate.Identifier.ID.ToString() + orderNumber)
			{
				Thickness = ((double)propTable[WidthKey]).MilimetersToMeters(),
				MaterialNo = pltMaterial
			};

			List<TSG.Point> points = new List<TSG.Point>();
			foreach (TSM.ContourPoint point in node.Contour.ContourPoints)
			{
				points.Add(point);
			}

			WM.Vector3D proLcsYT = points[1].ToMediaPoint() - points[0].ToMediaPoint();
			proLcsYT.Normalize();
			Vector3D vYT = proLcsYT.ToIndoVector3D();

			proLcsYT = points[1].ToMediaPoint() - points[0].ToMediaPoint();
			WM.Vector3D proLcsYT2 = points[2].ToMediaPoint() - points[1].ToMediaPoint();

			WM.Vector3D crossProduct = WM.Vector3D.CrossProduct(proLcsYT, proLcsYT2);
			if (crossProduct.Length == 0)
			{
				crossProduct = proLcsYT2;
			}
			WM.Vector3D proLcsXT = crossProduct;
			proLcsXT *= CssUnitScale;
			proLcsXT.Normalize();
			Vector3D vZT = proLcsXT.ToIndoVector3D();
			Vector3D vXT = vZT * vYT;


			plate.OriginNo = Model.GetPointId(points[0]);

			plate.LocalCoordinateSystem = new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
			{
				VecX = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = vXT.DirectionX,
					Y = vXT.DirectionY,
					Z = vXT.DirectionZ
				},
				VecY = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = vYT.DirectionX,
					Y = vYT.DirectionY,
					Z = vYT.DirectionZ
				},
				VecZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = vZT.DirectionX,
					Y = vZT.DirectionY,
					Z = vZT.DirectionZ
				}
			};


			Matrix44 pltMatrix = new Matrix44(new WM.Point3D()
			{
				X = points[0].X.MilimetersToMeters(),
				Y = points[0].Y.MilimetersToMeters(),
				Z = points[0].Z.MilimetersToMeters()
			},
			vXT, vYT, vZT
			);

			WM.Point3D firstPt = new WM.Point3D
			{
				X = points[0].X.MilimetersToMeters(),
				Y = points[0].Y.MilimetersToMeters(),
				Z = points[0].Z.MilimetersToMeters()
			};

			firstPt = pltMatrix.TransformToLCS(firstPt);

			System.Windows.Point prevStartPoint = new System.Windows.Point(firstPt.X, firstPt.Y);

			Region2D region = new Region2D
			{
				Outline = new PolyLine2D
				{
					StartPoint = new Point2D
					{
						X = prevStartPoint.X,
						Y = prevStartPoint.Y
					}
				}
			};

			plate.Geometry = region;

			for (int i = 1; i < points.Count; i++)
			{
				WM.Point3D pt = new WM.Point3D
				{
					X = points[i].X.MilimetersToMeters(),
					Y = points[i].Y.MilimetersToMeters(),
					Z = points[i].Z.MilimetersToMeters(),
				};

				pt = pltMatrix.TransformToLCS(pt);
				System.Windows.Point newStartPoint = new System.Windows.Point(pt.X, pt.Y);
				region.Outline.Segments.Add(new LineSegment2D() { EndPoint = new Point2D() { X = newStartPoint.X, Y = newStartPoint.Y } });
			}
			//add last point
			WM.Point3D ptLast = new WM.Point3D
			{
				X = points[0].X.MilimetersToMeters(),
				Y = points[0].Y.MilimetersToMeters(),
				Z = points[0].Z.MilimetersToMeters(),
			};

			ptLast = pltMatrix.TransformToLCS(ptLast);
			System.Windows.Point lastPoint = new System.Windows.Point(ptLast.X, ptLast.Y);
			region.Outline.Segments.Add(new LineSegment2D() { EndPoint = new Point2D() { X = lastPoint.X, Y = lastPoint.Y } });

			return plate;
		}
	}
}