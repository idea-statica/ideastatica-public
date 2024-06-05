using CI.Geometry3D;
using IdeaRS.OpenModel.Geometry2D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures.Catalogs;
using TS = Tekla.Structures;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using WM = System.Windows.Media.Media3D;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class PlateImporter : BaseImporter<IIdeaPlate>
	{
		private static readonly string WidthKey = "WIDTH";
		internal const double vectorCompareTollerance = 1E-04;

		public PlateImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaPlate Create(string id)
		{
			var ideaObject = CreateInternal(id);
			if (ideaObject != null)
			{
				Model.CacheCreatedObject(new StringIdentifier<IIdeaPlate>(id), ideaObject);
			}
			return ideaObject;
		}

		private IIdeaPlate CreateInternal(string id)
		{
			PlugInLogger.LogInformation($"PlateImporter create '{id}'");
			var item = Model.GetItemByHandler(id);

			//BooleanPart OperativePart id aways point again on booleanPart so now rewrite item by operative part (real ContourPlate/beam)
			if (item is TSM.BooleanPart boolean)
			{
				item = boolean.OperativePart;
			}

			if (item is Tekla.Structures.Model.ContourPlate part)
			{
				PlugInLogger.LogInformation($"PlateImporter create contour plate'{id}'");
				return CreatePlateFromSolid(part);
			}
			else if (item is TSM.Beam beamPart)
			{
				PlugInLogger.LogInformation($"PlateImporter create plate from beam '{id}'");
				return AddBeamPart(beamPart);
			}
			else if (item is TSM.PolyBeam polyBeamPart)
			{
				PlugInLogger.LogInformation($"PlateImporter create plate from beam '{id}'");
				return AddBeamPart(polyBeamPart);
			}

			return null;
		}

		internal WM.Vector3D ToMediaVector(TSG.Vector src)
		{
			return new WM.Vector3D(src.X, src.Y, src.Z);
		}

		private Plate AddBeamPart(TSM.Beam beam)
		{
			LibraryProfileItem profileItem = new LibraryProfileItem();
			profileItem.Select(beam.Profile.ProfileString);
			switch (profileItem.ProfileItemType)
			{
				case ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN:
					{
						ParametricProfileItem paramProfileItem = new ParametricProfileItem();
						if (paramProfileItem.Select(beam.Profile.ProfileString))
						{
							ProfileItem.ProfileItemTypeEnum paramProfItemType = paramProfileItem.ProfileItemType;

							if (paramProfItemType == ProfileItem.ProfileItemTypeEnum.PROFILE_PL)
							{
								return CreatePlateFromSolid(beam);
							}

						}

						PlugInLogger.LogInformation($"PlateImporter AddBeamPart beam profile is not plate '{beam.Profile.ProfileString}'");
						return null;
					}
				default:
					PlugInLogger.LogInformation($"PlateImporter AddBeamPart beam profile is not plate '{profileItem.ProfileItemType}'");
					return null;
			}
		}

		private Plate AddBeamPart(TSM.PolyBeam beam)
		{
			LibraryProfileItem profileItem = new LibraryProfileItem();
			profileItem.Select(beam.Profile.ProfileString);
			switch (profileItem.ProfileItemType)
			{
				case ProfileItem.ProfileItemTypeEnum.PROFILE_UNKNOWN:
					{
						return CreatePlateFromSolid(beam);

					}
				default:
					PlugInLogger.LogInformation($"PlateImporter AddBeamPart polybeam profile is not plate '{profileItem.ProfileItemType}'");
					return null;
			}
		}

		private Plate CreatePlateFromSolid(TSM.Part part)
		{
			PlugInLogger.LogInformation($"PlateImporter CreatePlateFromSolid");
			string pltMaterial = part.Material.MaterialString;

			var propTable = new Hashtable();
			part.GetDoubleReportProperties(new ArrayList
				{
					WidthKey
				}, ref propTable);

			Plate plate = new Plate(part.Identifier.GUID.ToString())
			{
				Thickness = ((double)propTable[WidthKey]).MilimetersToMeters(),
				MaterialNo = pltMaterial
			};

			var solid = part.GetSolid(TSM.Solid.SolidCreationTypeEnum.NORMAL);
			TSG.CoordinateSystem partCs = part.GetCoordinateSystem();
			TSG.Vector axisZ = TSG.Vector.Cross(partCs.AxisX, partCs.AxisY);
			WM.Vector3D pltAxisZ = ToMediaVector(axisZ);
			pltAxisZ.Normalize();

			var faceEnum = solid.GetFaceEnumerator();

			TS.Solid.Face foundFace = null;
			while (faceEnum.MoveNext())
			{
				WM.Vector3D tempFaceNormal = ToMediaVector(faceEnum.Current.Normal);
				tempFaceNormal.Normalize();

				if (tempFaceNormal.IsEqual(pltAxisZ, vectorCompareTollerance))
				{
					foundFace = faceEnum.Current;
					break;
				}
			}

			if (foundFace == null)
			{
				foundFace = faceEnum.Current;
			}

			if (foundFace == null)
			{
				//not found face skip it
				return null;
			}

			var loopEnumerator = foundFace.GetLoopEnumerator();
			if (!loopEnumerator.MoveNext())
			{
				PlugInLogger.LogInformation($"PlateImporter CreatePlateFromSolid Invalid loop");
				throw new ArgumentException("Invalid loop");
			}

			var firstLoop = loopEnumerator.Current;

			var vertexEnumerator = firstLoop.GetVertexEnumerator();

			List<TSG.Point> points = new List<TSG.Point>();
			while (vertexEnumerator.MoveNext())
			{
				points.Add(vertexEnumerator.Current);
			}
			TSG.Point origin = partCs.Origin;
			if (part is TSM.Beam beam)
			{
				var centLine = beam.GetCenterLine(false);
				TSG.Point centLineStart = centLine[0] as TSG.Point;
				TSG.Point centLineEnd = centLine[centLine.Count - 1] as TSG.Point;

				origin = new TSG.Point((centLineStart.X + centLineEnd.X) / 2, (centLineStart.Y + centLineEnd.Y) / 2, (centLineStart.Z + centLineEnd.Z) / 2);
			}

			plate.OriginNo = Model.GetPointId(origin);

			WM.Vector3D pltAxisX = ToMediaVector(partCs.AxisX);
			pltAxisX.Normalize();

			WM.Vector3D pltAxisY = ToMediaVector(partCs.AxisY);
			pltAxisY.Normalize();

			plate.LocalCoordinateSystem = new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
			{
				VecX = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = pltAxisX.X,
					Y = pltAxisX.Y,
					Z = pltAxisX.Z
				},
				VecY = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = pltAxisY.X,
					Y = pltAxisY.Y,
					Z = pltAxisY.Z
				},
				VecZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = pltAxisZ.X,
					Y = pltAxisZ.Y,
					Z = pltAxisZ.Z

				}
			};


			Matrix44 pltMatrix = new Matrix44(new WM.Point3D()
			{
				X = origin.X.MilimetersToMeters(),
				Y = origin.Y.MilimetersToMeters(),
				Z = origin.Z.MilimetersToMeters()
			},
			pltAxisX.ToIndoVector3D(),
			pltAxisY.ToIndoVector3D(),
			pltAxisZ.ToIndoVector3D()
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
				Outline = new PolyLine2D()
			};

			region.Outline.StartPoint = new Point2D
			{
				X = prevStartPoint.X,
				Y = prevStartPoint.Y
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
			var lastPoint = new System.Windows.Point(ptLast.X, ptLast.Y);
			region.Outline.Segments.Add(new LineSegment2D() { EndPoint = new Point2D() { X = lastPoint.X, Y = lastPoint.Y } });

			return plate;
		}
	}
}
